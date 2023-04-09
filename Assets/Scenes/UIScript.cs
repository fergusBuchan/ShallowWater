using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIScript : MonoBehaviour
{

    [SerializeField]
    public ObjectManager objectManager;

    private Label simName;
    private Label instructions;

    // Start is called before the first frame update
    void Start()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        instructions = root.Q<Label>("Instructions");
        simName = root.Q<Label>("SimulationName");
        simName.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        switch (objectManager.iterator % 3) {
            case 0:
                simName.text = "A";
                simName.style.backgroundColor = new Color(100,0,0,1);
                instructions.style.display = DisplayStyle.None;
                break;
            case 1:
                simName.text = "B";
                simName.style.backgroundColor = new Color(0, 100, 0, 1);
                instructions.style.display = DisplayStyle.None;
                break;
            case 2:
                simName.text = "C";
                simName.style.backgroundColor = new Color(0, 0, 100, 1);
                instructions.style.display = DisplayStyle.None;
                break;
        }

    }
}
