//public var heightmap : Texture2D; 
//public var darkImg : boolean = false;
//public var threshold : float = 0.8;

public var Vertices : Vector3[];
public var ShiftedVertices : Vector3[];
public var Triangles : int[];
public var UV : Vector2[];

public var Height : float;
Height = 2.0f;

var mesh : Mesh;

public var ReceivedData : Vector3[];

function Start() 
{
    print("Leggow");
    
}

function Update()
{
    //get input from OpenCv in vertices and indexes
    //currently simulated by mockdata
    if (ReceiveData() != null) {
        ReceivedData = ReceiveData();
    } 
    else 
    {
        print("No data Received");
    }
    //convert the 2d array to 3d array and add extra vertices for 3d mode
    if (ShiftPlane(ReceivedData) != null)
    {
        ShiftedVertices = ShiftPlane(ReceivedData);
    }
    else
    {
        print("Cant shift planes");
    }
    //calculate faces 'n edges
    CalculateEdges(ShiftedVertices);

    //apply mesh
    if (mesh == null) 
    {
        CreateMesh();
    } 
    else 
    {
        ApplyMesh();
    }

}

//Method used for simulating received data
function ReceiveData() : Vector3[]
    {
        var ReceivedVertices : Vector3[] = new Vector3[50];
        ReceivedVertices[0] = new Vector3(0, 0, 0);
        ReceivedVertices[1] = new Vector3(20, 0, 0); 
        ReceivedVertices[2] = new Vector3(20, 0, 20);
        ReceivedVertices[3] = new Vector3(40, 0, 10);
        ReceivedVertices[4] = new Vector3(42, 0, 24);
        return ReceivedVertices;
    }

    //pronounce as: two-dee-two-three-dee
    //shifts the incomming 2d data to a 3d plane by adding another layer of points on a plane with height Height
    function ShiftPlane(_2dPlane : Vector3[]) : Vector3[]
        {
            var Shifted : Vector3[];
            Shifted = _2dPlane;
            for (var i = 0; i < _2dPlane.length; i++)
            {
                var TempVec : Vector3 = _2dPlane[i] + new Vector3(0, Height, 0);
                Shifted[i+_2dPlane.length] = TempVec;
                print(TempVec);
            }
            return Shifted;
        }

        function CalculateEdges(vertices : Vector3[]) 
            {
    
            }

            //Creates a new Mesh
            function CreateMesh() 
            {
                mesh = new Mesh();
                GetComponent.<MeshFilter>().mesh = mesh;
            }

            //Apply the new vertexdata to the mesh
            function ApplyMesh() 
            {
                mesh.vertices = ShiftedVertices;
                mesh.uv = UV;
                mesh.triangles = Triangles;
            }

            /*public function update() {
                updateHeightMap();
                StartCoroutine(ApplyHeightmap);
            }
            
            function updateHeightMap()
            {
                //get input from openCV in vertices
                var mesh : Mesh = GetComponent.<MeshFilter>().mesh;
                var vertices : Vector3[] = mesh.vertices;
            
                for (var i = 0; i < vertices.Length; i++)
                    vertices[i] += Vector3.up * Time.deltaTime;
            
                // assign the local vertices array into the vertices array of the Mesh.
                mesh.vertices = vertices;
                mesh.RecalculateBounds();
                yield WaitForSeconds(1.0f);
            }
            
            public function ApplyHeightmap () 
            {
                
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
                 
             
                if (terrainWidth != heightmapWidth || heightmapHeight != heightmapWidth) 
                {
                    // Resize using nearest-neighbor scaling if texture has no filtering
                    if (heightmap.filterMode == FilterMode.Point) 
                    {
                        var dx : float = parseFloat(heightmapWidth)/terrainWidth;
                        var dy : float = parseFloat(heightmapHeight)/terrainWidth;
                        var y : int;
                        for (y = 0; y < terrainWidth; y++) 
                        {
                            if (y%20 == 0) 
                            {
                                EditorUtility.DisplayProgressBar("Resize", "Calculating texture", Mathf.InverseLerp(0.0, terrainWidth, y));
                            }
                            var thisY = parseInt(dy*y)*heightmapWidth;
                            var yw = y*terrainWidth;
                            var x : int;
                            for (x = 0; x < terrainWidth; x++) 
                            {
                                map[yw + x] = mapColors[thisY + dx*x];
                            }
                        }
                    }
                        // Otherwise resize using bilinear filtering
                    else 
                    {
                        var ratioX = 1.0/(parseFloat(terrainWidth)/(heightmapWidth-1));
                        var ratioY = 1.0/(parseFloat(terrainWidth)/(heightmapHeight-1));
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
                        if(heightmapData[y,x] < threshold && !darkImg) {
                            heightmapData[y, x] = 1;
                        }
                        else if(heightmapData[y,x] > threshold && darkImg) {
                            heightmapData[y, x] = 1;
                        }
                        else {
                            heightmapData[y, x] = 0;
                        }
                        //Debug.Log(terrain.GetHeight(y, x));
                    }
                }
                terrain.SetHeights(0, 0, heightmapData);
                yield WaitForSeconds(1.0f);
            }*/