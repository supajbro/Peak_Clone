using UnityEngine;

public class PlayerEyes : MonoBehaviour
{
    [Header("Material")]
    [SerializeField] private Material _originalMat;
    [SerializeField] private SkinnedMeshRenderer _mesh;
    private Material _mat;

    [Header("Expression Values")]
    [SerializeField] private float _timer = 0f;
    [SerializeField] private float _minChangeTimer = 1f;
    [SerializeField] private float _maxChangeTimer = 10f;
    private float _expressionTimer = 0f;

    [Header("Expression Types")]
    [SerializeField, Tooltip("Expressions player will change to automatically")] private int[] _regularExpressions = { 1, 2, 3, 4 };

    private bool _overrideExpression = false;

    private void Start()
    {
        _mat = new Material(_originalMat);
        _expressionTimer = Random.Range(_minChangeTimer, _maxChangeTimer);
    }

    private void Update()
    {
        ExpressionUpdate();
    }

    /// <summary>
    /// Randomly chooses between regular expressions
    /// </summary>
    private void ExpressionUpdate()
    {
        if(_overrideExpression)
        {
            _timer = 0f;
            return;
        }

        _timer += Time.deltaTime;

        if(_timer > _expressionTimer)
        {
            _timer = 0f;
            _expressionTimer = Random.Range(_minChangeTimer, _maxChangeTimer);

            int randomValue = _regularExpressions[Random.Range(0, _regularExpressions.Length)];
            _mat.SetFloat("_Expression", randomValue);
            _mesh.material = _mat;
        }
    }

    /// <summary>
    /// If you want to manually override user expression (i.e. punch eyes) set this
    /// </summary>
    /// <param name="index">Index of expression</param>
    public void SetExpression(int index)
    {
        _overrideExpression = true;
        _mat.SetFloat("_Expression", index);
        _mesh.material = _mat;
    }
}
