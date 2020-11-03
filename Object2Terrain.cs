using UnityEngine;
using UnityEditor;

public class Object2Terrain : EditorWindow
{
    [MenuItem("Custom/Edit/Object to Terrain &q")]
    static void DoObject2Terrain()
    {
        BeginTransfer();
    }

    static void BeginTransfer()
    {
        if (!Selection.activeGameObject)
        {
            Debug.Log("[Object2Terrain] No object is selected. ");
            return;
        }

        // 選択中のオブジェクトを取得する
        GameObject target = Selection.activeGameObject;

        // オブジェクトにアタッチされているコンポーネントを型を指定して取得する
        // ここでは MeshCollider を取得する。アタッチされていない場合は null が返される
        MeshCollider target_col = target.GetComponent<MeshCollider>();
        if (!target_col)
        {
            Debug.Log("[Object2Terrain] No collider is detected in selection.");
            return;
        }

        // 選択中のオブジェクトのバウンディングボックスの情報を取得する
        Bounds targetbounds = target_col.bounds;
        
        // ※ Terrainがシーンに一つしか無いことを前提
        TerrainData terrain_data = Terrain.activeTerrain.terrainData;
        Undo.RecordObject(Terrain.activeTerrain, "Object to Terrain");
        Bounds bounds = Terrain.activeTerrain.GetComponent<TerrainCollider>().bounds;

        int cHMWidth = terrain_data.heightmapWidth;         // 地形の幅と高さを取得する(この高さは高度のことではない)
        int cHMHeight = terrain_data.heightmapHeight;       // ２次元配列で管理されている Heightmap のサイズ

        float cTerrainHeight = terrain_data.size.y;         // Terrain Heightの値

        float[,] heightmap = new float[cHMWidth, cHMHeight];                     // 計算後のHeightmapを入れる領域
        float[,] originmap = terrain_data.GetHeights(0, 0, cHMWidth, cHMHeight); // 現在の Heightmap を取得(全領域)

        // 走査開始位置はTerrainの原点とする。
        // 高度は選択中のオブジェクトの最上部より少し上に固定する。
        // 光の方向は、下向き。
        Vector3 ray_origin = new Vector3(bounds.min.x, targetbounds.max.y + 1, bounds.min.z);
        Ray ray = new Ray(ray_origin, Vector3.down);

        // 光の長さは選択中のオブジェクトの底辺まで届く長さとする。
        float cRayDistance = targetbounds.size.y + 1;
        float cTerrainY = Terrain.activeTerrain.GetPosition().y;

        // 走査するポイントの縦横の数
        float nextWidth = bounds.size.z / cHMWidth;
        float nextHeight = bounds.size.x / cHMHeight;

        // 衝突結果を受け取る領域
        RaycastHit hit = new RaycastHit();

        for (int i = 0; i < cHMWidth; i++)
        {
            for (int j = 0; j < cHMHeight; j++)
            {
                // 選択中のオブジェクトより高い位置から、Terrainの領域全体に光を降らせて、
                // 選択中のオブジェクトのコライダーに衝突した場所の高さを記録する。
                bool result = target_col.Raycast(ray, out hit, cRayDistance);
                if (result)
                {
                    // 高度0から衝突位置の差を、新しい高度とする。
                    // (高度はTerrain Heightに対しての割合)
                    heightmap[i, j] = (hit.point.y - cTerrainY) / cTerrainHeight;
                }
                else
                {
                    // 衝突しない場合は、現在のTerrainの高さを維持する。
                    heightmap[i, j] = originmap[i, j];
                }
                // 次の行へ
                ray_origin.x += nextHeight;
                ray.origin = ray_origin;
            }
            // 次の列へ
            ray_origin.x = bounds.min.x;
            ray_origin.z += nextWidth;
            ray.origin = ray_origin;
        }
        terrain_data.SetHeights(0, 0, heightmap);
    }
}