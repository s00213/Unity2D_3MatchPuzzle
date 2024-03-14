using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreElement : MonoBehaviour
{
    public TextMeshProUGUI usernameText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI pointText;

    public void NewScoreElement (string _username, int _level, int _point)
    {
        usernameText.text = _username;
		levelText.text = _level.ToString();
		pointText.text = _point.ToString();
    }
}
