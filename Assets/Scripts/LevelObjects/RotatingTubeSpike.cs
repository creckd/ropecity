using UnityEngine;

public class RotatingTubeSpike : LevelObject {

	private Material tubeMat;

	private void Start() {
		tubeMat = GetComponentInChildren<MeshRenderer>().material;
		//tubeMat.SetVector("_TilingDirection", data.flowDirection);
		BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
		if (Mathf.Abs(transform.position.z) > 5f)
			boxCollider.enabled = false;
	}

	protected override void Update() {
		base.Update();
		tubeMat.SetFloat("_WorldLiquidHeight", transform.position.y);
	}

}
