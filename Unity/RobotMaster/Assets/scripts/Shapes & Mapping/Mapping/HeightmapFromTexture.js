@MenuItem ("Terrain/Heightmap From Texture")

static function ApplyHeightmap () 
{
    var heightmap : Texture2D = Selection.activeObject as Texture2D;
    if (heightmap == null) 
    { 
        EditorUtility.DisplayDialog("No texture selected", "Please select a texture.", "Cancel"); 
        return; 
    }
    Undo.RegisterUndo (Terrain.activeTerrain.terrainData, "Heightmap From Texture");
 
    var terrain = Terrain.activeTerrain.terrainData;
    var heightmapWidth = heightmap.width;
    var heightmapHeight = heightmap.height;
    var terrainWidth = terrain.heightmapWidth;
    var heightmapData = terrain.GetHeights(0, 0, terrainWidth, terrainWidth);
    var mapColors = heightmap.GetPixels();
    var map = new Color[terrainWidth * terrainWidth];
    var threshold : float = 0.8; 
 
    if (terrainWidth != heightmapWidth || heightmapHeight != heightmapWidth) 
    {
        // Resize using nearest-neighbor scaling if texture has no filtering
        if (heightmap.filterMode == FilterMode.Point) 
        { 
            var dx: float;
            var dy: float;
            dx = parseFloat(heightmapWidth)/terrainWidth;
            dy = parseFloat(heightmapHeight)/terrainWidth;
            var Y: int;
            Y = 0;
            for (Y = 0; Y < terrainWidth; Y++) 
            {
                if (Y%20 == 0) 
                {
                    EditorUtility.DisplayProgressBar("Resize", "Calculating texture", Mathf.InverseLerp(0.0, terrainWidth, Y));
                }
                var thisY = parseInt(dy*Y)*heightmapWidth;
                var yw = Y*terrainWidth;
                var X: int;
                X = 0;
                for (X = 0; X < terrainWidth; X++) 
                {
                    map[yw + X] = mapColors[thisY + dx*X];
                }
            }
        }
            // Otherwise resize using bilinear filtering
        else 
        {
            var ratioX = 1.0/(parseFloat(terrainWidth)/(heightmapWidth-1));
            var ratioY = 1.0/(parseFloat(terrainWidth)/(heightmapHeight-1));
            var y : int;
            for (y = 0; y < terrainWidth; y++) 
            {
                if (y%20 == 0) 
                {
                    EditorUtility.DisplayProgressBar("Resize", "Calculating texture", Mathf.InverseLerp(0.0, terrainWidth, y));
                }
                var yy = Mathf.Floor(y*ratioY);
                var y1 = yy*heightmapWidth;
                var y2 = (yy+1)*heightmapWidth;
                yw = y*terrainWidth;
                var x : int;
                for (x = 0; x < terrainWidth; x++) 
                {
                    var xx = Mathf.Floor(x*ratioX);
 
                    var bl = mapColors[y1 + xx];
                    var br = mapColors[y1 + xx+1]; 
                    var tl = mapColors[y2 + xx];
                    var tr = mapColors[y2 + xx+1];
 
                    var xLerp = x*ratioX-xx;
                    map[yw + x] = Color.Lerp(Color.Lerp(bl, br, xLerp), Color.Lerp(tl, tr, xLerp), y*ratioY-yy);
                }
            }
        }
        EditorUtility.ClearProgressBar();
    }
    else 
    {
        // Use original if no resize is needed
        map = mapColors;
    }
 
    // Assign texture data to heightmap
    for (y = 0; y < terrainWidth; y++) 
    {
        for (x = 0; x < terrainWidth; x++) 
        {
            heightmapData[y,x] = map[y*terrainWidth+x].grayscale;
            if(heightmapData[y,x] < threshold) {
                heightmapData[y, x] = 1;
            }
            else {
                heightmapData[y, x] = 0;
            }
            //Debug.Log(terrain.GetHeight(y, x));
        }
    }
    terrain.SetHeights(0, 0, heightmapData);
}