using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugGeneration : MonoBehaviour
{
    [SerializeField] Text debugText = null;

    [SerializeField] ThreadGeneration threadGeneration = null;

    void UpdateText()
	{
        debugText.text = "Thread Generation\n" +
            "   State : " + threadGeneration.ThreadState;
	}
}
