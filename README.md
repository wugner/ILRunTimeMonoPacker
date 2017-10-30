# ILRunTimeMonoPacker
An extend for ILRuntime in Unity3D to simply code MonoBehaviour like class inside hot dll and use it as common MonoBehaviour.<br/>
<br/>
Add an attribute [ILHotMonoProxyAttribute] to your dll class and use project [ReflectInfoGenerator] to generate a info file for your dll.<br/>
Copy this generated config file to your unity project, add ILMonoProxy component to your game object, you can select the dll class in drop down list, and inspector will display it like common MonoBehaviour.
<br/><br/>
Based on ILRuntime 1.3<br/>
https://github.com/Ourpalm/ILRuntime

