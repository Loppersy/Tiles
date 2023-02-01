using System.Collections;

using System.Collections.Generic;

using UnityEngine;

using UnityEditor;

using TMPro;

[ExecuteInEditMode]

public class getParentName : MonoBehaviour

{



    void LateUpdate()

    {

        var parentName = transform.parent.name;

        TextMeshProUGUI textmeshPro = GetComponent<TextMeshProUGUI>();

        textmeshPro.SetText(parentName);

    }
}