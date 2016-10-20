public var Vertices : Vector3[];
public var ShiftedVertices : Vector3[];
public var Triangles : int[];
public var UV : Vector2[];

public var Height : float;
Height = 2.0f;

public var mesh : Mesh;

public var ReceivedData : Vector3[];

function Start() 
{
    print("Leggow");
    CreateMesh();
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
    CalculateTriangles(ShiftedVertices);

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
        var ReceivedVertices : Vector3[] = new Vector3[5];
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
            //Shifted = _2dPlane;
            Shifted = new Vector3[_2dPlane.length * 2];
            for (var i = 0; i < _2dPlane.length; i++) {
                Shifted[i] = _2dPlane[i];
                var TempVec : Vector3 = _2dPlane[i] + new Vector3(0, Height, 0);
                Shifted[i+_2dPlane.length] = TempVec;
                print(TempVec);
            }
            return Shifted;
        }

        function CalculateTriangles(vertices : Vector3[]) 
            {
            
            }

            //Creates a new Mesh
            function CreateMesh() {
                print("create mesh");
                mesh = new Mesh();
                if(GetComponent.<MeshFilter>() != null)
                    GetComponent.<MeshFilter>().mesh = mesh;
            }

            //Apply the new vertexdata to the mesh
            function ApplyMesh() {
                print("apply mesh");
                mesh.vertices = ShiftedVertices;
                mesh.uv = UV;
                mesh.triangles = Triangles;
            }