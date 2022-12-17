using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;
using System.Collections.Generic;

namespace OAKForUnity
{
    public class lowpassfilter
    {
        float alpha = 0.9f;
        bool isInstantiate = false;
        float storedvalue = 0;
        float rawvalue = 0;
        float output = 0;
        
        public lowpassfilter(float a)
        {
            alpha = a;
        }
        public float apply(float input)
        {
            if (isInstantiate)
            {
                output = alpha*input + (1f - alpha)*storedvalue;
            }
            else
            {
                output = input;
                isInstantiate = true;
            }
            rawvalue = input;
            storedvalue = output;
            return output;
        }
    }
    public class BodyPoseCanvasController : MonoBehaviour
    {
        // public attributes
        
        // Pipeline for texture binding
        public DaiBodyPose pipeline;
        
        [Header("UI Binding")] 
        public Image colorCameraImage;
        public TMPro.TextMeshProUGUI bodyPoseResults;
        
        private bool _init = false;

        Vector3[] landmarks;
        Vector2[] landmarks2D;
        // Here are all the body segments
        GameObject originalGameObject;
        GameObject Armature;
        GameObject RootBone;
        GameObject Base;
        GameObject Abdomen;
        GameObject Chest;
        GameObject ShoulderBoneLeft;
        GameObject ShoulderBoneRight;
        GameObject ShoulderLeft;
        GameObject ShoulderRight;
        GameObject ElbowLeft;
        GameObject ElbowRight;
        GameObject WristLeft;
        GameObject WristRight;
        GameObject HipBoneLeft;
        GameObject HipBoneRight;
        GameObject HipLeft;
        GameObject HipRight;
        GameObject KneeLeft;
        GameObject KneeRight;
        GameObject Head;

        //Lowpassfilter
        lowpassfilter wristL1;
        lowpassfilter wristL2;
        lowpassfilter wristR1;
        lowpassfilter wristR2;

        lowpassfilter elbowL1;
        lowpassfilter elbowL2;
        lowpassfilter elbowR1;
        lowpassfilter elbowR2;

        lowpassfilter shoulderL1;
        lowpassfilter shoulderL2;
        lowpassfilter shoulderR1;
        lowpassfilter shoulderR2;

        lowpassfilter hipL1;
        lowpassfilter hipL2;
        lowpassfilter hipR1;
        lowpassfilter hipR2;

        public TMPro.TMP_Dropdown m_Dropdown;
        public GameObject dropdownObject;
        int DropdownValue;

        GameObject[] gameObjectArrayAbdoOut;
        GameObject[] gameObjectArrayAbdoIn;
        GameObject[] gameObjectArrayArms;
        GameObject[] gameObjectArrayHead;

        // Start is called before the first frame update
        void Start()
        {
            // Fatch all the gameobjects with tags
            gameObjectArrayAbdoOut = GameObject.FindGameObjectsWithTag ("AbdominalOuter");
            gameObjectArrayAbdoIn = GameObject.FindGameObjectsWithTag ("AbdominalInner");
            gameObjectArrayArms = GameObject.FindGameObjectsWithTag ("Arms");
            gameObjectArrayHead = GameObject.FindGameObjectsWithTag ("HeadNeck");
            //Initialize dropdown menu
            m_Dropdown = dropdownObject.GetComponent<TMPro.TMP_Dropdown>();
            
            m_Dropdown.onValueChanged.AddListener(delegate {
            DropdownValueChanged(m_Dropdown);
            });

            // Initialize all the body segments
            landmarks = new Vector3[17];
            landmarks2D = new Vector2[17];
            originalGameObject = GameObject.Find("Muscle (1)");
            Armature = originalGameObject.transform.GetChild(34).gameObject;
            RootBone = Armature.transform.GetChild(0).gameObject;
            Base = RootBone.transform.GetChild(0).gameObject;
            
            HipLeft = RootBone.transform.GetChild(1).gameObject;
            HipRight = RootBone.transform.GetChild(2).gameObject;

            

            Abdomen = Base.transform.GetChild(0).gameObject;
            Chest = Abdomen.transform.GetChild(0).gameObject;
            
            ShoulderLeft = Abdomen.transform.GetChild(7).gameObject;
            ShoulderRight = Abdomen.transform.GetChild(8).gameObject;
            ElbowLeft = ShoulderLeft.transform.GetChild(2).gameObject;
            ElbowRight = ShoulderRight.transform.GetChild(2).gameObject;
            WristLeft = ElbowLeft.transform.GetChild(2).gameObject;
            WristRight = ElbowRight.transform.GetChild(2).gameObject;

            //Initialize lowpass filters
            wristL1 = new lowpassfilter(0.5f);
            wristL2 = new lowpassfilter(0.5f);
            wristR1 = new lowpassfilter(0.5f);
            wristR2 = new lowpassfilter(0.5f);

            elbowL1 = new lowpassfilter(0.5f);
            elbowL2 = new lowpassfilter(0.5f);
            elbowR1 = new lowpassfilter(0.5f);
            elbowR2 = new lowpassfilter(0.5f);

            shoulderL1 = new lowpassfilter(0.5f);
            shoulderL2 = new lowpassfilter(0.5f);
            shoulderR1 = new lowpassfilter(0.5f);
            shoulderR2 = new lowpassfilter(0.5f);

            hipL1 = new lowpassfilter(0.5f);
            hipL2 = new lowpassfilter(0.5f);
            hipR1 = new lowpassfilter(0.5f);
            hipR2 = new lowpassfilter(0.5f);
            
        }
        // When the dropdown menu was chosen, change the corresponding visulization
        void DropdownValueChanged(TMPro.TMP_Dropdown change)
        {
            print(change.value);
            DropdownValue = change.value;
            //Outer
            if (DropdownValue == 3)
            {
                
 
                foreach(GameObject go in gameObjectArrayArms)
                {
                    //go.SetActive (false);
                    Color tempcolor;
                    tempcolor = go.GetComponent<Renderer> ().material.color;
                    go.GetComponent<Renderer> ().material.color = new Color(tempcolor.r,tempcolor.g,tempcolor.b,0.2f);
                }
 
                foreach(GameObject go in gameObjectArrayAbdoOut)
                {
                    //go.SetActive (false);
                    Color tempcolor;
                    tempcolor = go.GetComponent<Renderer> ().material.color;
                    go.GetComponent<Renderer> ().material.color = new Color(tempcolor.r,tempcolor.g,tempcolor.b,1f);
                }
                foreach(GameObject go in gameObjectArrayAbdoIn)
                {
                    //go.SetActive (false);
                    Color tempcolor;
                    tempcolor = go.GetComponent<Renderer> ().material.color;
                    go.GetComponent<Renderer> ().material.color = new Color(tempcolor.r,tempcolor.g,tempcolor.b,0.2f);
                }
                foreach(GameObject go in gameObjectArrayHead)
                {
                    //go.SetActive (false);
                    Color tempcolor;
                    tempcolor = go.GetComponent<Renderer> ().material.color;
                    go.GetComponent<Renderer> ().material.color = new Color(tempcolor.r,tempcolor.g,tempcolor.b,0.2f);
                }
            }
            //Inner
            if (DropdownValue == 4)
            {
                
 
                foreach(GameObject go in gameObjectArrayArms)
                {
                    //go.SetActive (false);
                    Color tempcolor;
                    tempcolor = go.GetComponent<Renderer> ().material.color;
                    go.GetComponent<Renderer> ().material.color = new Color(tempcolor.r,tempcolor.g,tempcolor.b,0.2f);
                }
 
                foreach(GameObject go in gameObjectArrayAbdoOut)
                {
                    //go.SetActive (false);
                    Color tempcolor;
                    tempcolor = go.GetComponent<Renderer> ().material.color;
                    go.GetComponent<Renderer> ().material.color = new Color(tempcolor.r,tempcolor.g,tempcolor.b,0f);
                }
                foreach(GameObject go in gameObjectArrayAbdoIn)
                {
                    //go.SetActive (false);
                    Color tempcolor;
                    tempcolor = go.GetComponent<Renderer> ().material.color;
                    go.GetComponent<Renderer> ().material.color = new Color(tempcolor.r,tempcolor.g,tempcolor.b,1f);
                }
                foreach(GameObject go in gameObjectArrayHead)
                {
                    //go.SetActive (false);
                    Color tempcolor;
                    tempcolor = go.GetComponent<Renderer> ().material.color;
                    go.GetComponent<Renderer> ().material.color = new Color(tempcolor.r,tempcolor.g,tempcolor.b,0.2f);
                }
            }
            else if (DropdownValue == 0)
            {
                
 
                foreach(GameObject go in gameObjectArrayArms)
                {
                    //go.SetActive (true);
                    Color tempcolor;
                    tempcolor = go.GetComponent<Renderer> ().material.color;
                    go.GetComponent<Renderer> ().material.color = new Color(tempcolor.r,tempcolor.g,tempcolor.b,1f);
                }
 
                foreach(GameObject go in gameObjectArrayAbdoOut)
                {
                    //go.SetActive (true);
                    Color tempcolor;
                    tempcolor = go.GetComponent<Renderer> ().material.color;
                    go.GetComponent<Renderer> ().material.color = new Color(tempcolor.r,tempcolor.g,tempcolor.b,1f);
                }
                foreach(GameObject go in gameObjectArrayAbdoIn)
                {
                    //go.SetActive (false);
                    Color tempcolor;
                    tempcolor = go.GetComponent<Renderer> ().material.color;
                    go.GetComponent<Renderer> ().material.color = new Color(tempcolor.r,tempcolor.g,tempcolor.b,0.2f);
                }
                foreach(GameObject go in gameObjectArrayHead)
                {
                    //go.SetActive (true);
                    Color tempcolor;
                    tempcolor = go.GetComponent<Renderer> ().material.color;
                    go.GetComponent<Renderer> ().material.color = new Color(tempcolor.r,tempcolor.g,tempcolor.b,1f);
                }
            }
            else if (DropdownValue == 1)
            {
                foreach(GameObject go in gameObjectArrayArms)
                {
                    //go.SetActive (false);
                    Color tempcolor;
                    tempcolor = go.GetComponent<Renderer> ().material.color;
                    go.GetComponent<Renderer> ().material.color = new Color(tempcolor.r,tempcolor.g,tempcolor.b,0.3f);
                }
                foreach(GameObject go in gameObjectArrayAbdoIn)
                {
                    //go.SetActive (false);
                    Color tempcolor;
                    tempcolor = go.GetComponent<Renderer> ().material.color;
                    go.GetComponent<Renderer> ().material.color = new Color(tempcolor.r,tempcolor.g,tempcolor.b,0.2f);
                }
                foreach(GameObject go in gameObjectArrayAbdoOut)
                {
                    //go.SetActive (false);
                    Color tempcolor;
                    tempcolor = go.GetComponent<Renderer> ().material.color;
                    go.GetComponent<Renderer> ().material.color = new Color(tempcolor.r,tempcolor.g,tempcolor.b,0.3f);
                }
                
                foreach(GameObject go in gameObjectArrayHead)
                {
                    //go.SetActive (true);
                    Color tempcolor;
                    tempcolor = go.GetComponent<Renderer> ().material.color;
                    go.GetComponent<Renderer> ().material.color = new Color(tempcolor.r,tempcolor.g,tempcolor.b,1f);
                }
            }
            else if (DropdownValue == 2)
            {
                foreach(GameObject go in gameObjectArrayArms)
                {
                    //go.SetActive (true);
                    Color tempcolor;
                    tempcolor = go.GetComponent<Renderer> ().material.color;
                    go.GetComponent<Renderer> ().material.color = new Color(tempcolor.r,tempcolor.g,tempcolor.b,1f);
                }
                foreach(GameObject go in gameObjectArrayAbdoIn)
                {
                    //go.SetActive (false);
                    Color tempcolor;
                    tempcolor = go.GetComponent<Renderer> ().material.color;
                    go.GetComponent<Renderer> ().material.color = new Color(tempcolor.r,tempcolor.g,tempcolor.b,0.2f);
                }
                foreach(GameObject go in gameObjectArrayAbdoOut)
                {
                    //go.SetActive (false);
                    Color tempcolor;
                    tempcolor = go.GetComponent<Renderer> ().material.color;
                    go.GetComponent<Renderer> ().material.color = new Color(tempcolor.r,tempcolor.g,tempcolor.b,0.3f);
                }
                
                foreach(GameObject go in gameObjectArrayHead)
                {
                    //go.SetActive (false);
                    Color tempcolor;
                    tempcolor = go.GetComponent<Renderer> ().material.color;
                    go.GetComponent<Renderer> ().material.color = new Color(tempcolor.r,tempcolor.g,tempcolor.b,0.3f);
                }
            }
            
        }
        // Binding Textures. Wait to have pipeline running.
        private void Init()
        {
            // Texture2D binding
            colorCameraImage.material.mainTexture = pipeline.colorTexture;
            _init = true;
        }

        // Update is called once per frame
        void Update()
        {
            
            
            // parse the pose
            if (pipeline.deviceRunning && !_init) Init();
            bodyPoseResults.text = pipeline.bodyPoseResults;
            var json = JSON.Parse(pipeline.bodyPoseResults);
            if (json == null) return;
            var arr = json["landmarks"];
            print(json);

            //for (int i = 0; i<17; i++) landmarks[i] = Vector3.zero;
            
            foreach(JSONNode obj in arr)
            {
                int index = -1;
                float x = 0.0f,y = 0.0f,z = 0.0f;
                float kx = 0.0f, ky = 0.0f;

                index = obj["index"];
                x = obj["location.x"];
                y = obj["location.y"];
                z = obj["location.z"];

                kx = obj["xpos"];
                ky = obj["ypos"];
                
                if (index != -1) 
                {
                    if (x!=0 && y!=0 && z!=0)
                    {
                        //landmarks[index] = new Vector3(x/1000,y/1000,z/1000);
                        landmarks2D[index] = new Vector2(kx,ky);
                        
                    }
                }
            }
            

            //Lowpass filter to filter out the jitter effect
            landmarks2D[9] = new Vector2(wristL1.apply(landmarks2D[9][0]),wristL2.apply(landmarks2D[9][1]));
            landmarks2D[10] = new Vector2(wristR1.apply(landmarks2D[10][0]),wristR2.apply(landmarks2D[10][1]));

            landmarks2D[7] = new Vector2(elbowL1.apply(landmarks2D[7][0]),elbowL2.apply(landmarks2D[7][1]));
            landmarks2D[8] = new Vector2(elbowR1.apply(landmarks2D[8][0]),elbowR2.apply(landmarks2D[8][1]));

            landmarks2D[5] = new Vector2(shoulderL1.apply(landmarks2D[5][0]),shoulderL2.apply(landmarks2D[5][1]));
            landmarks2D[6] = new Vector2(shoulderR1.apply(landmarks2D[6][0]),shoulderR2.apply(landmarks2D[6][1]));

            landmarks2D[11] = new Vector2(hipL1.apply(landmarks2D[11][0]),hipL2.apply(landmarks2D[11][1]));
            landmarks2D[12] = new Vector2(hipR1.apply(landmarks2D[12][0]),hipR2.apply(landmarks2D[12][1]));

            //Some distance calculation
            Vector2 hipmid2D;
            hipmid2D = (landmarks2D[11] + landmarks2D[12])/2;
            //print(hipmid2D);
            Vector2 shouldermid2D = (landmarks2D[5] + landmarks2D[6])/2;
            float shoulderHalfdis = getDistance2D(landmarks2D[5],landmarks2D[6])/2;
            float hipHalfdis = getDistance2D(landmarks2D[11],landmarks2D[12])/2;
            float hip2shoulder = getDistance2D(hipmid2D,shouldermid2D);
            float shoulder2elbowL = getDistance2D(landmarks2D[5],landmarks2D[7]);
            float shoulder2elbowR = getDistance2D(landmarks2D[6],landmarks2D[8]);
            float elbow2wristL = getDistance2D(landmarks2D[7],landmarks2D[9]);
            //Debug.Log(landmarks2D[9]);
            //Debug.Log(elbow2wristL);
            float elbow2wristR = getDistance2D(landmarks2D[8],landmarks2D[10]);
            //print(new Vector3(5.4f-hipmid2D.x*0.06854f,8.4f-hipmid2D.y*0.06792f,0));
            Armature.transform.position = new Vector3(5.4f-hipmid2D.x*0.06854f,8.4f-hipmid2D.y*0.06792f,19.51f); //need to change
            
            //Determine the root position of the whole model
            RootBone.transform.localPosition = new Vector3(-0.0341f+hipmid2D.x*0.000484f,-0.0595f+hipmid2D.y*0.000484f,0f);
            RootBone.transform.localScale = new Vector3(hipHalfdis*0.05f,hipHalfdis*0.05f,hipHalfdis*0.05f);
            //Calculate the lean angle (forward and backward)
            Vector3 leanAngle = new Vector3(-(90*shoulderHalfdis/hipHalfdis)+130,0,-180);
            RootBone.transform.localEulerAngles = leanAngle;

            //Calculate the lean angle (left and right)
            Base.transform.localEulerAngles = -getAngleLeft(shouldermid2D,hipmid2D);
            Base.transform.eulerAngles = new Vector3(Base.transform.eulerAngles.x,-180,Base.transform.eulerAngles.z);




            //Calculate the angle of the elbow and the shoulder
            ElbowLeft.transform.eulerAngles = getAngleRight(landmarks2D[10],landmarks2D[8]);
            ElbowRight.transform.eulerAngles = getAngleLeft(landmarks2D[9],landmarks2D[7]);
           

            ShoulderLeft.transform.eulerAngles = getAngleRight(landmarks2D[8],landmarks2D[6]);
            ShoulderRight.transform.eulerAngles = getAngleLeft(landmarks2D[7],landmarks2D[5]);
        }

        float getDepth(float x1, float y1, float x2, float y2, float length)
        {
            float depth;
            depth = Mathf.Sqrt(Mathf.Pow(length,2) - Mathf.Pow(x1 - x2,2) - Mathf.Pow(y1 - y2,2));
            return depth;
        }
        float getDistance2D (Vector2 point1, Vector2 point2)
        {
            float distance;
            distance = Mathf.Sqrt( Mathf.Pow(point1.x - point2.x,2) + Mathf.Pow(point1.y - point2.y,2));
            return distance;
        }

        Vector3 getAngleLeft(Vector2 point1, Vector2 point2)
        {
            Vector3 angle;
            angle.x = 0;
            if (point1.x > point2.x)
            {
                angle.z = (Mathf.Atan((point1.y - point2.y)/(point1.x - point2.x))*180/Mathf.PI) + 90f;
                
            }
            else
            {
                angle.z = (Mathf.Atan((point1.y - point2.y)/(point1.x - point2.x))*180/Mathf.PI) - 90f;
                
            }
            angle.y = 0;
            
            return angle + new Vector3(0f, 180f, 0f);
        }
        Vector3 getAngleRight(Vector2 point1, Vector2 point2)
        {
            Vector3 angle;
            angle.x = 0;
            
            if (point1.x > point2.x)
            {
                angle.z = (Mathf.Atan((point1.y - point2.y)/(point1.x - point2.x))*180/Mathf.PI) + 90f;
                
            }
            else
            {
                angle.z = (Mathf.Atan((point1.y - point2.y)/(point1.x - point2.x))*180/Mathf.PI) - 90f;
                
            }
            angle.y = 0;
            
            return angle;
        }
    }
}