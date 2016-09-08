using UnityEngine;
using System.Collections;

using ProtoBuf;
using protospike;
using System.IO;

public class ProtobuffTest : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        Person loadedPerson;
        using (var file = File.OpenRead("john.dat"))
        {
            loadedPerson = Serializer.Deserialize<Person>(file);
        }

        Debug.Log(loadedPerson);

        Debug.Log(loadedPerson.name);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
