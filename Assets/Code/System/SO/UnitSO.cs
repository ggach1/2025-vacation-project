using UnityEngine;

namespace Code.System.SO
{
    [CreateAssetMenu(fileName = "Unit", menuName = "SO/Unit")]
    public class UnitSO : ScriptableObject
    {
        [Header("¿Ø¥÷ ¡§∫∏")]
        [field : SerializeField] public Sprite Icon { get; private set; }
        [field : SerializeField] public string Name { get; private set; }
        [field: SerializeField] public int Cost { get; private set; }
        [field: SerializeField] public string Tip { get; private set; }

        [Header("¿Ø¥÷ ø¿∫Í¡ß∆Æ")]
        [field: SerializeField] public GameObject UnitObj { get; private set; }
    }
}