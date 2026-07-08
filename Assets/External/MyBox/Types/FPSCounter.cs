using MyBox;
using UnityEngine;

public class FPSCounter : MonoBehaviour {
    public bool EditorOnly;

    [SerializeField] private float _updateInterval = 1f;
    [SerializeField] private int _targetFrameRate = 30;

#pragma warning disable 0649
    [SerializeField] private Anchor _anchor;
    [SerializeField] private int _xOffset;
    [SerializeField] private int _yOffset;
#pragma warning restore 0649

    /// <summary>
    /// Skip some time at start to skip performance drop on game start
    /// and produce more accurate Avg FPS
    /// </summary>
    private float _idleTime = 2;

    private float _elapsed;
    private int _frames;
    private int _quantity;
    private float _fps;
    private float _averageFps;

    private Color _goodColor;
    private Color _okColor;
    private Color _badColor;

    private float _okFps;
    private float _badFps;

    private Rect _rect1;
    private Rect _rect2;

    private void Awake() {
        if (this.EditorOnly && !Application.isEditor) return;

        this._goodColor = new Color(.4f, .6f, .4f);
        this._okColor = new Color(.8f, .8f, .2f, .6f);
        this._badColor = new Color(.8f, .6f, .6f);

        var percent = this._targetFrameRate / 100;
        var percent10 = percent * 10;
        var percent40 = percent * 40;
        this._okFps = this._targetFrameRate - percent10;
        this._badFps = this._targetFrameRate - percent40;

        var xPos = 0;
        var yPos = 0;
        var linesHeight = 40;
        var linesWidth = 90;
        if (this._anchor == Anchor.LeftBottom || this._anchor == Anchor.RightBottom) yPos = Screen.height - linesHeight;
        if (this._anchor == Anchor.RightTop || this._anchor == Anchor.RightBottom) xPos = Screen.width - linesWidth;
        xPos += this._xOffset;
        yPos += this._yOffset;
        var yPos2 = yPos + 18;
        this._rect1 = new Rect(xPos, yPos, linesWidth, linesHeight);
        this._rect2 = new Rect(xPos, yPos2, linesWidth, linesHeight);

        this._elapsed = this._updateInterval;
    }

    private void Update() {
        if (this.EditorOnly && !Application.isEditor) return;

        if (this._idleTime > 0) {
            this._idleTime -= Time.deltaTime;
            return;
        }

        this._elapsed += Time.deltaTime;
        ++this._frames;

        if (this._elapsed >= this._updateInterval) {
            this._fps = this._frames / this._elapsed;
            this._elapsed = 0;
            this._frames = 0;
        }

        this._quantity++;
        this._averageFps += (this._fps - this._averageFps) / this._quantity;
    }

    private void OnGUI() {
        if (this.EditorOnly && !Application.isEditor) return;

        var defaultColor = GUI.color;
        var color = this._goodColor;
        if (this._fps <= this._okFps || this._averageFps <= this._okFps) color = this._okColor;
        if (this._fps <= this._badFps || this._averageFps <= this._badFps) color = this._badColor;
        GUI.color = color;
        GUI.Label(this._rect1, "FPS: " + (int)this._fps);
        //GUI.Label(_rect2, "Avg FPS: " + (int)_averageFps);
        GUI.color = defaultColor;
    }

    private enum Anchor {
        LeftTop, LeftBottom, RightTop, RightBottom
    }
}
