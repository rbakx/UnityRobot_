# Unity
The unity project can be run like a usual unity project. It's only dependency is the [C# networking library][networking].

## Unit Testing
[Unity Test Tools] are used for testing unity scripts/behaviour. More information on how to write tests [here][utest1], and [here (MonoBehaviours)][utest2]. Official documentation can be found [here][utest3].

Unity (C#/mono) networking tests can be found [here][utest4]. These tests can be run from the Unity editor (Window -> Editor Test Runner -> Run All).

![Unity unit tests](https://github.com/sangun/UnityRobot/wiki/images/unity-unit-tests.png)

[networking]: ../../networking/README.md
[Unity Test Tools]: https://www.assetstore.unity3d.com/en/#!/content/13802
[utest1]: https://blogs.unity3d.com/2014/07/28/unit-testing-at-the-speed-of-light-with-unity-test-tools/
[utest2]: https://blogs.unity3d.com/2014/06/03/unit-testing-part-2-unit-testing-monobehaviours/
[utest3]: https://bitbucket.org/Unity-Technologies/unitytesttools/wiki/Home
[utest4]: Assets/scripts/tests/Editor/networking
