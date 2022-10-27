using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
[System.Serializable]
public struct state
{
    public string name;
    public int[] onYPositions;
    public int reward;
}

public class SlotsMachine : MonoBehaviour
{
    public float maxSpinTime;
    public float stopTime;
    public float spinSpeed;
    public float snapTime;
    public float instantSnapDistance;
    public state[] states;
    public int length;
    public Transform[] rows;
    public TextMeshProUGUI reward;
    private bool spinning;
    // Start is called before the first frame update
    void Awake()
    {
        spinning = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator SlotsSpin()
    {
        if (!spinning)
        {
            yield return new WaitForEndOfFrame();
            spinning = true;
            reward.text = "";
            float t = Time.time;
            float[] speed = new float[rows.Length];
            for(int i = 0; i< rows.Length; i++)
            {
                speed[i] = Random.Range(0.8f, 1.2f);
            }
            while (Time.time - t < maxSpinTime)
            {
                if (Input.GetMouseButtonDown(0)) break;
                float deltaTime = Time.deltaTime;
                for (int i = 0; i < rows.Length; i++)
                {
                    rows[i].Translate(Vector3.down * speed[i] * spinSpeed * deltaTime);
                    if (rows[i].position.y <= -length) rows[i].Translate(Vector3.up * length);
                }
                yield return new WaitForEndOfFrame();
            }
            t = Time.time;
            while (Time.time - t < stopTime)
            {
                float deltaTime = Time.deltaTime;
                for (int i = 0; i < rows.Length; i++)
                {
                    rows[i].Translate(Vector3.down * (speed[i]-(speed[i]*(Time.time-t)/stopTime)) * spinSpeed * deltaTime);
                    if (rows[i].position.y <= -length) rows[i].Translate(Vector3.up * length);
                }
                yield return new WaitForEndOfFrame();
            }
            t = Time.time;
            float[] snapdistance = new float[length];
            for (int i = 0; i < rows.Length; i++)
            {
                snapdistance[i] = -rows[i].position.y % 1;
                if (snapdistance[i] >= 0.5f) snapdistance[i] = -1 + snapdistance[i];
            }
            while (Time.time - t < snapTime)
            {
                    float deltaTime = Time.deltaTime;
                for (int i = 0; i < rows.Length; i++)
                {
                    rows[i].Translate(Vector3.up * snapdistance[i] / snapTime * deltaTime);
                }
                yield return new WaitForEndOfFrame();
            }
            for (int i = 0; i < rows.Length; i++)
            {
                float snap = rows[i].position.y % 1 < 0.5f ? rows[i].position.y % 1 : (rows[i].position.y % 1) - 1;
                rows[i].Translate(Vector3.down * (rows[i].position.y - Mathf.Round(rows[i].position.y)));
            }
            reward.text = "try again";
            spinning = false;
            foreach (state s in states)
            {
                bool WinState = true;
                for (int i = 0; i < rows.Length; i++)
                    if (!s.onYPositions.Contains((int)rows[i].position.y)) { WinState = false; break; }
                if (WinState)
                {
                    Debug.Log($"You Won A {s.name}");
                    reward.text = $"Reward: {s.reward}$!";
                    break;
                }

            }
        }
    }
    private void OnMouseDown()
    {
        StartCoroutine(SlotsSpin());
    }
}
