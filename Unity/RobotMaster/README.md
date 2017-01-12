# Unity
The unity project can be run like a usual unity project. It's only dependency is the [C# networking library][networking].

Unity needs to know an ip address and port, for the brokers and vision module to connect to. Unity needs to know this address so that the connections can be opened on the correct network interface. The ip address can be set on the RobotRegister component under the mother->UnitControl gameobject.

![unity-hierarchy-unitcontrol][unity-hierarchy-unitcontrol]
![unity-unitcontrol-rr-component][unity-unitcontrol-rr-component]

## Unit Testing
[Unity Test Tools] are used for testing unity scripts/behaviour. More information on how to write tests [here][utest1], and [here (MonoBehaviours)][utest2]. Official documentation can be found [here][utest3].

Unity (C#/mono) networking tests can be found [here][utest4]. These tests can be run from the Unity editor (Window -> Editor Test Runner -> Run All).

Note that the "TEST_TCPAsioConnection" test is meant for testing the connection between the C# and c++ networking libraries. This test will always pass since it is internally disabled. To enable/disable this test, use the "Testing" menu in unity's menu bar.

![Unity unit tests][Unity unit tests]

[networking]: ../../networking/README.md
[unity-hierarchy-unitcontrol]: https://github.com/sangun/UnityRobot/wiki/images/unity-hierarchy-unitcontrol.png
[unity-unitcontrol-rr-component]: https://github.com/sangun/UnityRobot/wiki/images/unity-unitcontrol-rr-component.png
[Unity Test Tools]: https://www.assetstore.unity3d.com/en/#!/content/13802
[utest1]: https://blogs.unity3d.com/2014/07/28/unit-testing-at-the-speed-of-light-with-unity-test-tools/
[utest2]: https://blogs.unity3d.com/2014/06/03/unit-testing-part-2-unit-testing-monobehaviours/
[utest3]: https://bitbucket.org/Unity-Technologies/unitytesttools/wiki/Home
[utest4]: Assets/scripts/tests/Editor/networking
[Unity unit tests]: https://github.com/sangun/UnityRobot/wiki/images/unity-unit-tests.png
