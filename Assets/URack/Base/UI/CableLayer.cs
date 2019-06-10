using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Eidetic.URack.Base.UI
{
    public class CableLayer : IMGUIContainer
    {
        public CableLayer()
        {
            name = "CableLayer";
        }

        public static void DrawCable(Vector2 startPoint, Vector2 endPoint, Color color)
        {
            Vector2 startTangent = startPoint;
            if (startPoint.x < endPoint.x) startTangent.x = Mathf.LerpUnclamped(startPoint.x, endPoint.x, 0.7f);
            else startTangent.x = Mathf.LerpUnclamped(startPoint.x, endPoint.x, -0.7f);

            Vector2 endTangent = endPoint;
            if (startPoint.x > endPoint.x) endTangent.x = Mathf.LerpUnclamped(endPoint.x, startPoint.x, -0.7f);
            else endTangent.x = Mathf.LerpUnclamped(endPoint.x, startPoint.x, 0.7f);

            Handles.DrawBezier(startPoint, endPoint, startTangent, endTangent, color, null, 5);
        }
    }
}
