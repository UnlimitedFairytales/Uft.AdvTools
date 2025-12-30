#nullable enable

using System;
using System.Collections.Generic;
using Uft.UnityUtils;
using Uft.UnityUtils.Common;

namespace Uft.AdvTools.Entities
{
    public class Param
    {
        // Static members

        public static readonly Eval Eval = new();

        public static void AssignIntParamFromExpression(string expression, AdvRoot advRoot)
        {
            var i = expression.IndexOf('=');
            if (i <= 0) throw new Exception("LHS is not found.");

            var lhs = expression[..i].Trim();
            if (!advRoot.ParamDictionary.ContainsKey(lhs)) throw new Exception("Undefined lhs.");

            var rhs = expression[(i + 1)..].Trim();
            rhs = ReplaceParam(rhs, advRoot.ParamDictionary);
            rhs = ToDataTableExpression(rhs);
            var result = Eval.EvaluateDoubleOrNull(rhs) ?? throw new Exception($"{nameof(Eval.EvaluateDoubleOrNull)} returns null");

            advRoot.ParamDictionary[lhs].Value = (int)result;
            DevLog.Log($"[{nameof(Param)}] {lhs}={advRoot.ParamDictionary[lhs].Value} (Expression : {expression})");
        }

        public static bool EvaluateBoolean(string expression, AdvRoot advRoot)
        {
            var exp = ReplaceParam(expression, advRoot.ParamDictionary);
            exp = ToDataTableExpression(exp);
            var result = Eval.EvaluateBooleanOrNull(exp) ?? throw new Exception($"[{nameof(Param)}] {nameof(Eval)}.{nameof(Eval.EvaluateBooleanOrNull)} returns null");
            DevLog.Log($"[{nameof(Param)}] exp=({exp}), result={result} (Expression : {expression})");
            return result;
        }

        /// <summary>速度優先で string.Replace を使用しているため、変数名が部分一致で変換される危険があります。</summary>
        public static string ReplaceParam(string expression, Dictionary<string, Param> paramDictionary)
        {
            foreach (var param in paramDictionary)
            {
                expression = expression.Replace(param.Key, param.Value.Value.ToString("0"));
            }
            return expression;
        }

        public static string ToDataTableExpression(string expression)
        {
            expression = expression.Replace("!=", $" <> ");
            expression = expression.Replace("&&", $" {Eval.Keyword.AND} ");
            expression = expression.Replace("||", $" {Eval.Keyword.OR} ");
            return expression;
        }

        // Instance members

        // operator, Equals, GetHashCode, ToString

        public override string ToString() => $"{this.Label}={this.Value}";

        // Parameters

        public string Label { get; protected set; } // key

        // Status

        public int Value { get; set; }

        // Methods

        public Param(string label, int? value)
        {
            this.Label = label;
            this.Value = value ?? 0;
        }
    }
}
