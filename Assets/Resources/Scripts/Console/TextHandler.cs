using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TextHandler : MonoBehaviour
{
    public TMP_InputField inputField;
    public TextMeshProUGUI textField;

    public AudioSource audioSrc;
    public AudioClip characterWrite;

   /* public Lights lights;
    public Print printing;
    public FaxHandler fax;
    public DamageHandler damage;
*/
    public MapScreen mapHandler;
    private MapScreen map;

    public float writeDelay = 0.03f;

    private string gameLogs = "";

    private void Awake()
    {
        map = mapHandler.GetComponent<MapScreen>();
    }

   /* public void GetText()
    {
        string st = inputField.text;
        string[] expSt = st.Split(' ');

        inputField.text = "";

        if (st == "help")
        {
            Write("That's your help.\n");
        }
        else if (st == "clear")
        {
            gameLogs = "";
            textField.SetText(gameLogs);
        }
        else if (st == "lights")
        {
            StartCoroutine(lights.ToggleLights());
        }
        else if (expSt[0] == "print")
        {
            if (expSt[1] == "example")
            {
                printing.PrintPage("Lorem ipsum dolor sit amet", "Consectetur adipiscing elit. Etiam turpis enim, cursus eu lobortis sed, elementum ut velit. Fusce rhoncus ligula ac massa vehicula, non dapibus neque finibus. Quisque ac nulla imperdiet orci malesuada ultrices fringilla ac velit. Vestibulum neque libero, pellentesque ornare laoreet eget, ultrices non nulla. Maecenas vel urna faucibus magna mattis rhoncus. ");
            }
            else if (expSt[1] == "msg")
            {
                int id = int.Parse(expSt[2]) - 1;

                string[] faxPage = fax.GetFax(id);

                string title = faxPage[0];
                string content = faxPage[1];

                printing.PrintPage(title, content);
            }
        }
        else if (st == "inbox")
        {
            List<string[]> faxes = fax.GetFaxes();

            if (faxes.Count > 0)
            {
                string str = "Messages received:\n";

                for (int i = 0; i <= faxes.Count - 1; i++)
                {
                    str += "[" + (i + 1) + "] " + faxes[i][0] + '\n';
                }
                Write(str);
            }
            else
            {
                Write("No messages received.\n");
            }
        }
        else if (expSt[0] == "clear" && expSt[1] == "msg")
        {
            if (expSt.Length > 2)
            {
                fax.ClearFax(int.Parse(expSt[2]) - 1);
            }
            else
            {
                fax.ClearFaxes();
            }
        }
        else if(st == "dmg")
        {
            damage.GetDamage();
        }

    }
*/
    private void Write(string st)
    {
        StartCoroutine(WriteCore(st));
    }

    private IEnumerator WriteCore(string st)
    {
        string str = "> ";
        foreach (char ch in st)
        {
            str += ch;
            audioSrc.PlayOneShot(characterWrite);
            textField.SetText(gameLogs + str);
            yield return new WaitForSeconds(writeDelay);
        }
        gameLogs += str;
    }
}

