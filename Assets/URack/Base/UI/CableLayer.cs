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
        static CableLayer instance;
        public static CableLayer Instance
        {
            get
            {
                if (instance == null) instance = new CableLayer();
                return instance;
            }
        }

        static Dictionary<int, Cable> Cables = new Dictionary<int, Cable>();

        CableLayer() : base(OnGUI)
        {
            name = "CableLayer";
            pickingMode = PickingMode.Ignore;

            var outputPorts = URack.Instance.Rack.Modules
                .SelectMany(m => m.Ports)
                .Where(p => p.IsOutput && p.IsConnected);

            foreach (var port in outputPorts)
                DrawCable(port.GetHashCode(),
                    PortElement.PortElements[port].worldBound.center,
                    PortElement.PortElements[port.Connection].worldBound.center);

            MarkDirtyRepaint();
        }

        public static void OnGUI()
        {
            foreach (var cable in Cables.Values)
            {
                var start = cable.startPoint;
                var end = cable.endPoint;

                var distance = start - end;
                var tension = 0.4f;

                var slump = Mathf.Abs((1 - tension) * (150 + 1 * distance.x));

                var middle = new Vector2((start.x + end.x) / 2f, (start.y + end.y) / 2f + slump);

                //Vector2 startTangent = start;
                //if (start.x < end.x) startTangent.x = Mathf.LerpUnclamped(start.x, middle.x, 0.7f);
                //else startTangent.x = Mathf.LerpUnclamped(start.x, middle.x, -0.7f);

                //Vector2 middleEndTangent = middle;
                //if (start.x > end.x || start.y > end.y) middleEndTangent.x = Mathf.LerpUnclamped(middle.x, start.x, -0.7f);
                //else middleEndTangent.x = Mathf.LerpUnclamped(middle.x, start.x, 0.7f);

                //Vector2 middleStartTangent = middle;
                //if (start.x < end.x || start.y < end.y) middleStartTangent.x = Mathf.LerpUnclamped(middle.x, end.x, 0.7f);
                //else middleStartTangent.x = Mathf.LerpUnclamped(middle.x, end.x, -0.7f);

                //Vector2 endTangent = end;
                //if (start.x > end.x) endTangent.x = Mathf.LerpUnclamped(middle.x, end.x, -0.7f);
                //else endTangent.x = Mathf.LerpUnclamped(middle.x, end.x, 0.7f);

                //Handles.DrawBezier(start, middle, startTangent, middleEndTangent, Color.red, null, 5);
                //Handles.DrawBezier(middle, end, middleStartTangent, endTangent, Color.red, null, 5);

                Handles.color = new Color(255, 0, 0, .5f);
                Handles.DrawAAPolyLine(Texture2D.whiteTexture, 5, new Vector3[] { start, end });
            }
        }

        public void DrawCable(int hashCode, Vector2 startPoint, Vector2 endPoint) =>
            Cables[hashCode] = new Cable(startPoint, endPoint);

        public void RemoveCable(int hashCode) => Cables.Remove(hashCode);

        struct Cable
        {
            public Vector2 startPoint, endPoint;
            public Cable(Vector2 start, Vector2 end)
            {
                startPoint = start;
                endPoint = end;
            }
        }
    }
}
