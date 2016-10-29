// Spectrum2.cs
// ©2016 Team 95

using UnityEngine;

namespace Route95.World {

	/// <summary>
	/// Class to handle equalizer visualization.
	/// </summary>
    public class Spectrum2 : SingletonMonoBehaviour<Spectrum2> {

		#region Vars

		/// <summary>
		/// Number of points in graph.
		/// </summary>
		[SerializeField]
		[Tooltip("Number of points in graph.")]
        int _numberOfPoints = 20;

		/// <summary>
		/// Base height of visualization.
		/// </summary>
		[SerializeField]
		[Tooltip("Base height of visualization.")]
        float _height;

		/// <summary>
		/// Radius of visualization.
		/// </summary>
		[SerializeField]
		[Tooltip("Radius of visualization.")]
        float _radius;

		/// <summary>
		/// 
		/// </summary>
        Transform[] _objects;

		/// <summary>
		/// 
		/// </summary>
        Vector3[] _points;

		/// <summary>
		/// Scale of visualization.
		/// </summary>
		[SerializeField]
		[Tooltip("Scale of visualization.")]
        float _scale = 20f;

		/// <summary>
		/// Opacity of visualization.
		/// </summary>
		[SerializeField]
		[Tooltip("Opacity of visualization.")]
        float _opacity;

		/// <summary>
		/// Point fall rate.
		/// </summary>
		[SerializeField]
		[Tooltip("Point fall rate.")]
        float _fallRate;

		/// <summary>
		/// 
		/// </summary>
        float _minHeight;

		/// <summary>
		/// 
		/// </summary>
        float _maxHeight;

		/// <summary>
		/// Reference to LineRenderer.
		/// </summary>
		LineRenderer _lineRenderer;

		#endregion
		#region Unity Callbacks

		new void Awake() {
            base.Awake();

			// Init vars
			_lineRenderer = GetComponent<LineRenderer>();
            _minHeight = _height - _scale;
            _maxHeight = _height + _scale;

            // Initialize visualizer points
            _objects = new Transform[_numberOfPoints + 1];
            _points = new Vector3[_numberOfPoints + 1];
            for (int i = 0; i < _numberOfPoints + 1; i++) {
                float angle = i * Mathf.PI * 2f / _numberOfPoints;
                Vector3 pos = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * _radius;
                GameObject point = new GameObject("Point" + i);
                _objects[i] = point.transform;
                point.transform.position = pos;
                point.transform.SetParent(transform);
                _points[i] = point.transform.position;
            }
            _lineRenderer.SetVertexCount(_numberOfPoints + 1);
            _lineRenderer.SetPositions(_points);
        }

        // Update is called once per frame
        void Update() {
            if (DynamicTerrain.Instance.FreqData == null) return;

            for (int i = 0; i < _numberOfPoints + 1; i++) {
                _points[i].x = _objects[i].position.x;
                _points[i].z = _objects[i].position.z;
                //float y = height + Mathf.Log (terrain.freqData.GetDataPoint ((float)(i==0 ? (numberOfObjects-1) : i) / numberOfObjects)) * scale;
                float r = (float)(i == _numberOfPoints ? 0 : i) / _numberOfPoints;
                float y = Mathf.Clamp(_height + _scale * 10f * DynamicTerrain.Instance.FreqData.GetDataPoint(r), _minHeight, _maxHeight);
                if (y != float.NaN) {
                    if (y < _points[i].y && _points[i].y >= _minHeight + _fallRate)
                        _points[i].y -= _fallRate;
                    else _points[i].y = y;
                }

            }
            _lineRenderer.SetPositions(_points);
        }

		#endregion
		#region Methods

		public void SetColor (Color color) {
			color.a = _opacity;

            _lineRenderer.SetColors(color, color);
            _lineRenderer.material.color = color;
		}

		#endregion
	}
}
