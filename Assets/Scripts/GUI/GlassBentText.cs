using UnityEngine;
using UnityEngine.UI;

public class GlassBentText : Text {

	protected override void OnPopulateMesh(VertexHelper vh) {
		base.OnPopulateMesh(vh);

		for (int i = 0; i < vh.currentVertCount; i++) {
			UIVertex vert = UIVertex.simpleVert;
			vh.PopulateUIVertex(ref vert, i);
			Vector3 position = vert.position;

			//manipulate position
			float ratio = (float)position.x / (preferredWidth);
			float angle = ratio * 0.25f * Mathf.PI;

			Vector3 center = new Vector3(0f, -5f, 0f);
			position -= center;

			Matrix4x4 rotMax = new Matrix4x4(
			new Vector4(Mathf.Cos(angle), -Mathf.Sin(angle), 0, 0),
			new Vector4(Mathf.Sin(angle), Mathf.Cos(angle), 0, 0),
			new Vector4(0, 0, 1, 0),
			new Vector4(0, 0, 0, 1)
			);

			position = rotMax.MultiplyPoint(position);
			position += center;

			float mappedRatio = ratio * 2 * Mathf.PI;
			float cos = Mathf.Cos(mappedRatio);
			float sin = Mathf.Sin(mappedRatio);


			vert.position = position;
			vh.SetUIVertex(vert, i);
		}
	}
}