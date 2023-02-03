using System.Collections;

using System.Collections.Generic;

using UnityEngine;

using UnityEditor;

using TMPro;
using UnityEngine.UI;

[ExecuteInEditMode]

public class getParentName : MonoBehaviour

{



    void LateUpdate()

    {

        var parentName = transform.parent.name;

        TextMeshProUGUI textmeshPro = GetComponent<TextMeshProUGUI>();

        textmeshPro.SetText(parentName);

        Button parentButton = transform.parent.GetComponent<Button>();

        if (parentButton == null) return;
        textmeshPro.color = parentButton.interactable ? Color.white : new Color(255,255,255,0.2f);

    }
}