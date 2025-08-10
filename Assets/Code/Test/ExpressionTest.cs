using System;
using System.Linq.Expressions;
using UnityEngine;

namespace Code.Test
{
    public class  TestPlayer
    {
        public int hp;
        public string id;
        public float speed;
    }

    public class ExpressionTest : MonoBehaviour
    {
        //public Func<int, int> GetFunc()
        //{
        //    // x => 4 + x

        //    // 4
        //    ConstantExpression number4 = Expression.Constant(4);
        //    // x
        //    ParameterExpression param = Expression.Parameter(typeof(int), "x");
        //    // 4 + x
        //    BinaryExpression addExp = Expression.Add(number4, param);

        //    Expression<Func<int, int>> lambda = Expression.Lambda<Func<int, int>>(addExp, param);
        //    return lambda.Compile();
        //}

        //public Func<float, float, float> GetFunc()
        //{
        //    ParameterExpression param = Expression.Parameter(typeof(float), "a");
        //    ParameterExpression param1 = Expression.Parameter(typeof(float), "b");

        //    BinaryExpression addExp = Expression.Add(param, param1);

        //    var lambda = Expression.Lambda<Func<float, float, float>>(addExp, param, param1);
        //    return lambda.Compile();
        //}

        public Func<T, TMenber> CreateMemberGetter<T, TMenber>(string memberName)
        {
            // T => T.memberName
            var param = Expression.Parameter(typeof(T), "x");
            var memberExp = Expression.PropertyOrField(param, memberName);
            var lambda = Expression.Lambda<Func<T, TMenber>>(memberExp, param);
            return lambda.Compile();
        }

        [ContextMenu("Test")]
        private void Test()
        {
            TestPlayer p = new TestPlayer
            {
                hp = 100,
                id = "Player1",
                speed = 5f
            };
            Func<TestPlayer, int> pPlayerHpGetter = CreateMemberGetter<TestPlayer, int>("hp");
            Func<TestPlayer, float> pPlayerIdGetter = CreateMemberGetter<TestPlayer, float>("id");
            Func<TestPlayer, float> pPlayerSpeedGetter = CreateMemberGetter<TestPlayer, float>("speed");

            //var func = GetFunc();
            //float result = func.Invoke(10f, 20f);

            //Debug.Log(result);
        }
    }
}

