using UnityEngine;

public class DNATubeSpike : LevelObject {

	public MeshRenderer[] tubeMeshes;
	private Material sharedMat = null;

	private void Start() {
		//tubeMat.SetVector("_TilingDirection", data.flowDirection);
		sharedMat = Instantiate(tubeMeshes[0].material);
		for (int i = 0; i < tubeMeshes.Length; i++) {
			tubeMeshes[i].sharedMaterial = sharedMat;
		}
		BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
		if (Mathf.Abs(transform.position.z) > 5f)
			boxCollider.enabled = false;
	}

	protected override void Update() {
		base.Update();
		sharedMat.SetFloat("_WorldLiquidHeight", transform.position.y);
	}

}
