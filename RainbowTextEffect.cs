using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

/// <summary>
/// Class for applying wobble effect to each character in a TextMeshPro text surrouned by '~' characters.
/// Example usage: "Hello~ ~World~" will wobble the word "World".
/// </summary>
public class RainbowTextEffect : MonoBehaviour
{
    [SerializeField]
    private float wobbleSpeed = 1.0f;

    public Gradient rainbow;
    private Mesh mesh;
    private TMP_Text textMesh;
    private Vector3[] vertices;
    
    private List<bool> wobbleStates = new List<bool>();

    // Start is called before the first frame update
    private void Start()
    {
        textMesh = GetComponent<TMP_Text>();
        if (textMesh == null) Debug.LogError("No TMP_Text component found");

        SetTextWithWobble(textMesh.text);
    }

    /// <summary>
    /// Updates the character wobble effect.
    /// </summary>
    private void Update()
    {
        textMesh.ForceMeshUpdate();
        mesh = textMesh.mesh;
        vertices = mesh.vertices;

        var colors = mesh.colors;

        for (var i = 0; i < textMesh.textInfo.characterCount; i++)
        {
            if (i >= wobbleStates.Count)
                break;

            var wobbleState = wobbleStates[i];
            if (wobbleState)
            {
                var c = textMesh.textInfo.characterInfo[i];
                var index = c.vertexIndex;

                if (char.IsControl(c.character) || char.IsWhiteSpace(c.character) || c.isVisible == false)
                    continue;

                if (index < vertices.Length - 3)
                {
                    var offset = Wobble(Time.time * wobbleSpeed + i);

                    colors[index] = rainbow.Evaluate(Mathf.Repeat(Time.time + vertices[index].x * 0.001f, 1f));
                    colors[index + 1] = rainbow.Evaluate(Mathf.Repeat(Time.time + vertices[index + 1].x * 0.001f, 1f));
                    colors[index + 2] = rainbow.Evaluate(Mathf.Repeat(Time.time + vertices[index + 2].x * 0.001f, 1f));
                    colors[index + 3] = rainbow.Evaluate(Mathf.Repeat(Time.time + vertices[index + 3].x * 0.001f, 1f));

                    vertices[index] += offset;
                    vertices[index + 1] += offset;
                    vertices[index + 2] += offset;
                    vertices[index + 3] += offset;
                }
            }
        }

        mesh.vertices = vertices;
        mesh.colors = colors;
        textMesh.canvasRenderer.SetMesh(mesh);
    }

    /// <summary>
    /// Sets the text of the TMP_Text component with a wobble effect.
    /// Each character surrounded by '~' characters will have the wobble effect applied.
    /// </summary>
    /// <param name="text">The text to be set.</param>
    public void SetTextWithWobble(string text)
    {
        wobbleStates = new List<bool>();
        var wobbleState = false;
        var textToRender = new StringBuilder();

        foreach (var character in text)
        {
            if (character == '~')
            {
                wobbleState = !wobbleState;
                continue;
            }

            textToRender.Append(character);
            wobbleStates.Add(wobbleState);
        }

        textMesh.text = textToRender.ToString();
    }

    /// <summary>
    /// Sets the text of the TMP_Text component with a wobble effect.
    /// Each character surrounded by '~' characters will have the wobble effect applied.
    /// </summary>
    /// <param name="text">The text to be set.</param>
    private Vector3 Wobble(float time)
    {
        return new Vector3(Mathf.Sin(time * 3.3f), Mathf.Cos(time * 2.5f), 0f);
    }
}
