using UnityEngine;
using System.Collections;

using ProtoBuf;
using protospike;
using System.IO;

public class ProtobuffTest : MonoBehaviour
{

	// Use this for initialization
	void Start ()
	{
		Person john = new Person () {
			id = 4,
			name = "John Doe",
			email = "jdoe@example.com",
			phones = { new Person.PhoneNumber { number = "123454", type = Person.PhoneType.HOME } }
		};

		using (var file = File.Create ("john.dat")) {
			Serializer.Serialize<Person> (file, john);
		}

		Person loadedPerson;
		using (var file = File.OpenRead ("john.dat")) {
			loadedPerson = Serializer.Deserialize<Person> (file);
		}

		Debug.Log (loadedPerson);

		Debug.Log (loadedPerson.name);
		Debug.Log (loadedPerson.id);
		Debug.Log (loadedPerson.email);
		Debug.Log (string.Format("{0} {1}", loadedPerson.phones[0].type, loadedPerson.phones[0].number));
	}

	// Update is called once per frame
	void Update ()
	{

	}
}
