using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "APP_", menuName = "Scriptables/Apps/new App")]
public class AppBaseDataSO : ScriptableObject {
    public string appName;
    public List<AppType> AppTypes;
}