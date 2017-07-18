using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCubePos : MonoBehaviour {

	public Transform[] trans;

	public GameObject cubePrefab;

	public float spacing = 0.5f;

	public float size = 0.2f;

	public int cubeDim = 5; // for creating a lattice of cXcXc cubes

    public bool mouseControl = true;

    public Transform leftController;

	Transform [] cubes;
	public Transform sphere;

    public int sensitivity = 1000;






    Quaternion[,] rotHist;
	Vector3 [,] posHist;
	Vector3 [,] velHist;
	Vector3 [,] avelHist; //angular velocity hist



	Quaternion [] rotHistB;
	Vector3 [] posHistB;
	Vector3 [] velHistB;
	Vector3 [] avelHistB;


	public int histArraySize = 30;

	int cubeArrLength;

	void Start () {
		cubeArrLength = cubeDim * cubeDim * cubeDim;

		cubes = new Transform[cubeArrLength];


		posHist = new Vector3[cubeArrLength,histArraySize];
		rotHist = new Quaternion[cubeArrLength,histArraySize];		
		velHist = new Vector3[cubeArrLength,histArraySize];
		avelHist = new Vector3[cubeArrLength,histArraySize];


		posHistB = new Vector3[histArraySize];
		rotHistB = new Quaternion[histArraySize];
		velHistB = new Vector3[histArraySize];
		avelHistB = new Vector3[histArraySize];


		int u = 0;
		for(int i = 0; i < cubeDim; i ++){
			for(int j = 0; j < cubeDim; j ++){
				for(int k = 0; k < cubeDim; k ++){



					Vector3 pos = new Vector3 ((i - cubeDim/2) * spacing, (j - cubeDim/2) * spacing, (k - cubeDim/2) * spacing);

					Quaternion rot = Quaternion.Euler(new Vector3(Random.Range(0,360f),Random.Range(0,360f),Random.Range(0,360f)));

					GameObject obj = Instantiate (cubePrefab, pos, Quaternion.identity);

					obj.transform.parent = this.transform;

					cubes [u] = obj.transform;

					cubes [u].transform.localScale = new Vector3 (size, size, size);

					u++;

				}
			}
		}

		for (int i = 0; i < cubeArrLength; i++) {



			rotHist [i, histK] = Quaternion.identity;
			posHist [i, histK] = Vector3.zero;
			velHist [i, histK] = Vector3.zero;
			avelHist [i, histK] = Vector3.zero;



		}
		rotHistB [histK] = Quaternion.identity;
		posHistB [histK] = Vector3.zero;
		velHistB [histK] = Vector3.zero;
		avelHistB [histK] = Vector3.zero;

	}

	bool startRecord = false;
	
	void Update () {



		if (Input.GetKeyDown (KeyCode.R)) {
			Start ();
		}

		if (Input.GetKeyDown (KeyCode.Space)) {

			startRecord = true;

		}






		
	}

	int histK = 0;
	int histScroll = 0;

	float mousX0 = 0;

	void FixedUpdate(){

		if (histK >= histArraySize)
			histK = 0;
		

		if (startRecord) {

			for (int i = 0; i < cubeArrLength; i++) {

				Rigidbody rig = cubes [i].gameObject.GetComponent<Rigidbody> ();


				posHist [i, histK] = cubes [i].position;
				rotHist [i, histK] = cubes [i].rotation;
				velHist [i, histK] = rig.velocity;
				avelHist [i, histK] = rig.angularVelocity;


			}

			Rigidbody rigB = sphere.gameObject.GetComponent<Rigidbody> ();

			posHistB [histK] = sphere.position;
			rotHistB [histK] = sphere.rotation;
			velHistB [histK] = rigB.velocity;
			avelHistB [histK] = rigB.angularVelocity;


			histK++;

		}

		if (Input.GetKeyDown (KeyCode.Mouse0) || OVRInput.GetDown(OVRInput.Button.One)) {

			startRecord = false;

            if (mouseControl)
            {
                mousX0 = Input.mousePosition.x;
            }else
            {
                mousX0 = leftController.position.x;
            }

			for (int i = 0; i < cubeArrLength; i++) {


				Rigidbody rig = cubes [i].gameObject.GetComponent<Rigidbody> ();



				rig.velocity = Vector3.zero;
				rig.angularVelocity = Vector3.zero;


			}

			Rigidbody rigB = sphere.gameObject.GetComponent<Rigidbody> ();



			rigB.velocity = Vector3.zero;
			rigB.angularVelocity = Vector3.zero;

		}

		if (Input.GetKey (KeyCode.Mouse0) || OVRInput.Get(OVRInput.Button.One)) {

            if (mouseControl)
            {
                histScroll = (histK + Mathf.RoundToInt(Input.mousePosition.x - mousX0)) % histArraySize;
            }else
            {
                histScroll = (histK + Mathf.RoundToInt((leftController.position.x - mousX0) * sensitivity)) % histArraySize;
                print((leftController.position.x - mousX0)*sensitivity);

                print(histScroll);

            }

            while (histScroll < 0) {
				histScroll += histArraySize;
			}



            for (int i = 0; i < cubeArrLength; i++) {

				if (posHist [i, histScroll] != Vector3.zero) {
				
					cubes [i].position = posHist [i, histScroll];
					cubes [i].rotation = rotHist [i, histScroll];
				}

			}
			if (posHistB [histScroll] != Vector3.zero) {

				sphere.position = posHistB [histScroll];
				sphere.rotation = rotHistB [histScroll];
			}

		}

		if (Input.GetKeyUp (KeyCode.Mouse0) || OVRInput.GetUp(OVRInput.Button.One)) {

			histK = histScroll;

			startRecord = true;

			for (int i = 0; i < cubeArrLength; i++) {


				Rigidbody rig = cubes [i].gameObject.GetComponent<Rigidbody> ();

				if (posHist [i, histScroll] != Vector3.zero) {
					
					cubes [i].position = posHist [i, histScroll];
					cubes [i].rotation = rotHist [i, histScroll];

					rig.velocity = velHist [i, histScroll];
					rig.angularVelocity = avelHist [i, histScroll];
				}						




			}

			Rigidbody rigB = sphere.gameObject.GetComponent<Rigidbody> ();

			if (posHistB [histScroll] != Vector3.zero) {
				
				sphere.position = posHistB [histScroll];
				sphere.rotation = rotHistB [histScroll];

				rigB.velocity = velHistB [histScroll];
				rigB.angularVelocity = avelHistB [histScroll];
			}
		}


	}
}
