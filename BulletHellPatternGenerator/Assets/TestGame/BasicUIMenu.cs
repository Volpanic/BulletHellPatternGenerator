using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class BasicUIMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void OnEnable()
    {
        Time.timeScale = 0;
        selectedIndex = 0;

        for (int i = 0; i < MenuOptions.Count; i++)
        {
            if (i == 0) MenuOptions[i].Text.color = SelectedColor;
            else MenuOptions[i].Text.color = UnselectedColor;
        }
    }

    private void OnDisable()
    {
        Time.timeScale = 1;
    }

    [System.Serializable]
    public struct MenuOption
    {
        public TextMeshProUGUI Text;
        public UnityEvent Event;
    }

    public Color SelectedColor;
    public Color UnselectedColor;

    public List<MenuOption> MenuOptions;
    private int selectedIndex = 0;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            MenuOptions[selectedIndex].Text.color = UnselectedColor;
            selectedIndex++;
            selectedIndex %= MenuOptions.Count;
            MenuOptions[selectedIndex].Text.color = SelectedColor;
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            MenuOptions[selectedIndex].Text.color = UnselectedColor;
            selectedIndex--;
            if (selectedIndex < 0) selectedIndex = MenuOptions.Count - 1;
            MenuOptions[selectedIndex].Text.color = SelectedColor;
        }

        if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            MenuOptions[selectedIndex].Event.Invoke();
        }
    }
}
