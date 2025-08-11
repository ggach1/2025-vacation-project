using Code.System.SO;
using UnityEngine;

namespace Code.System
{
    public class UnitStorage : MonoBehaviour, ISystem
    {
        [Header("유닛 SO")]
        [SerializeField] UnitSO[] Units;

        [Header("각 코스트 별 유닛 종류")]
        [SerializeField] int UnitCost1 = 5;
        [SerializeField] int UnitCost2 = 4;
        [SerializeField] int UnitCost3 = 3;
        [SerializeField] int UnitCost4 = 2;
        [SerializeField] int UnitCost5 = 1;

        [Header("각 코스트 별 유닛 종류의 최대 갯수")]
        [SerializeField] int Unit1Cnt = 18;
        [SerializeField] int Unit2Cnt = 15;
        [SerializeField] int Unit3Cnt = 12;
        [SerializeField] int Unit4Cnt = 10;
        [SerializeField] int Unit5Cnt = 9;
    }
}