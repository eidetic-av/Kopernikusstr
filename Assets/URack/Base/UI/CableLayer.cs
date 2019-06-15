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
        public static readonly Vector2 PortMouseOffset = new Vector2(-5, -26);

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

        bool Redraw = false;

        CableLayer() : base(OnGUI)
        {
            name = "CableLayer";
            pickingMode = PickingMode.Ignore;

            RegisterCallback<AttachToPanelEvent>(e => Redraw = true);
        }

        public static void OnGUI()
        {
            if (Instance.Redraw)
            {
                var outputPorts = URack.Instance.Rack.Modules
                    .SelectMany(m => m.Ports)
                    .Where(p => p.IsOutput && p.IsConnected);

                foreach (var port in outputPorts)
                    Instance.DrawCable(port.GetHashCode(),
                        PortElement.PortElements[port].worldBound.center + PortMouseOffset,
                        PortElement.PortElements[port.Connection].worldBound.center + PortMouseOffset);
            }

            foreach (var cable in Cables.Values)
            {
                var thickness = 8;
                var color = new Color(255, 0, 0);

                var start = cable.startPoint;
                var end = cable.endPoint;

                //var distance = start - end;
                //var tension = 0.4f;

                //var slump = Mathf.Abs((1 - tension) * (150 + 1 * distance.x));

                //var middle = new Vector2((start.x + end.x) / 2f, (start.y + end.y) / 2f + slump);

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

                Handles.color = color;
                Handles.DrawAAPolyLine(Texture2D.whiteTexture, thickness, new Vector3[] { start, end });

                GUI.DrawTexture(new Rect(start.x - thickness / 2, start.y - thickness / 2, thickness, thickness),
                    Resources.Load<Texture2D>("Cap"), ScaleMode.ScaleToFit, true, 0, color, 0, 0);

                GUI.DrawTexture(new Rect(end.x - thickness / 2, end.y - thickness / 2, thickness, thickness),
                    Resources.Load<Texture2D>("Cap"), ScaleMode.ScaleToFit, true, 0, color, 0, 0);
            }
        }

        public void DrawCable(int hashCode, Vector2 startPoint, Vector2 endPoint)
        {
            Cables[hashCode] = new Cable(startPoint, endPoint);
            MarkDirtyRepaint();
        }

        public void RemoveCable(int hashCode)
        {
            if (Cables.ContainsKey(hashCode))
            {
                Cables.Remove(hashCode);
                MarkDirtyRepaint();
            }
        }

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
