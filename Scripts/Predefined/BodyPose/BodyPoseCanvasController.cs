/*
 * Canvas controller goal is decouple UI from pipeline (PredefinedBase) and unity device (OAKDevice)
 * In order to give some flexibility we're using Material as basic object with Texture2D
 * In that way we can use Image in Canvas UI or just Material in objects
 */

// using UnityEngine;
// using UnityEngine.UI;

// namespace OAKForUnity
// {
//     public class BodyPoseCanvasController : MonoBehaviour
//     {
//         // public attributes
        
//         // Pipeline for texture binding
//         public DaiBodyPose pipeline;
        
//         [Header("UI Binding")] 
//         public Image colorCameraImage;
//         public TMPro.TextMeshProUGUI bodyPoseResults;
        
//         private bool _init = false;

//         // Start is called before the first frame update
//         void Start()
//         {
//         }

//         // Binding Textures. Wait to have pipeline running.
//         private void Init()
//         {
//             // Texture2D binding
//             colorCameraImage.material.mainTexture = pipeline.colorTexture;
//             _init = true;
//         }

//         // Update is called once per frame
//         void Update()
//         {
//             if (pipeline.deviceRunning && !_init) Init();
//             bodyPoseResults.text = pipeline.bodyPoseResults;
//             print(pipeline.landmarks);
//         }
//     }
// }

/*
 * Canvas controller goal is decouple UI from pipeline (PredefinedBase) and unity device (OAKDevice)
 * In order to give some flexibility we're using Material as basic object with Texture2D
 * In that way we can use Image in Canvas UI or just Material in objects
 */

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

        // Start is called before the first frame update
        void Start()
        {
            landmarks = new Vector3[17];
            landmarks2D = new Vector2[17];
            originalGameObject = GameObject.Find("Muscle");
            Armature = originalGameObject.transform.GetChild(34).gameObject;
            RootBone = Armature.transform.GetChild(0).gameObject;
            Base = RootBone.transform.GetChild(0).gameObject;
            // HipBoneLeft = RootBone.transform.GetChild(1).gameObject;
            // HipBoneRight = RootBone.transform.GetChild(2).gameObject;
            HipLeft = RootBone.transform.GetChild(1).gameObject;
            HipRight = RootBone.transform.GetChild(2).gameObject;
            KneeLeft = HipLeft.transform.GetChild(0).gameObject;
            KneeRight = HipRight.transform.GetChild(0).gameObject;
            Abdomen = Base.transform.GetChild(0).gameObject;
            Chest = Abdomen.transform.GetChild(0).gameObject;
            // ShoulderBoneLeft = Abdomen.transform.GetChild(1).gameObject;
            // ShoulderBoneRight = Abdomen.transform.GetChild(2).gameObject;
            ShoulderLeft = Abdomen.transform.GetChild(7).gameObject;
            ShoulderRight = Abdomen.transform.GetChild(8).gameObject;
            ElbowLeft = ShoulderLeft.transform.GetChild(2).gameObject;
            ElbowRight = ShoulderRight.transform.GetChild(2).gameObject;
            WristLeft = ElbowLeft.transform.GetChild(2).gameObject;
            WristRight = ElbowRight.transform.GetChild(2).gameObject;

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
            
            //parse the pose
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
                        landmarks[index] = new Vector3(x/1000,y/1000,z/1000);
                        landmarks2D[index] = new Vector2(kx,ky);
                        //print(landmarks[index]);
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
            
            Base.transform.localPosition = new Vector3(0,0.001f,0);
            Base.transform.eulerAngles = -getAngleLeft(shouldermid2D,hipmid2D);
            
            Vector3 lefthip = landmarks[11];
            Vector3 righthip = landmarks[12];
            //print(HipBoneLeft.transform.position);
            Vector3 ShoulderLeftpos = landmarks[5];
            Vector3 ShoulderRightpos = landmarks[6];

            Vector3 elbowLeftpos = landmarks[7];;
            Vector3 elbowRightpos = landmarks[8];

            Vector3 wristLeftpos = landmarks[9];
            Vector3 wristRightpos = landmarks[10];

            Vector3 kneeLeftpos = landmarks[13];
            Vector3 kneeRightpos = landmarks[14];

            float mainDis = getDistance((lefthip+righthip)/2,(ShoulderLeftpos+ShoulderRightpos)/2);
            //print(mainDis);
            //RootBone.transform.position = (lefthip+righthip)/2;
            //RootBone.transform.position = new Vector3(RootBone.transform.position.x,RootBone.transform.position.y,0);
            //print(RootBone.transform.position);
            //Abdomen.transform.localPosition = new Vector3(0,(mainDis)*0.01f,0); // Y axis is the half of the link length between the hip and the shoulder
            Abdomen.transform.localPosition = new Vector3(0,hip2shoulder*0.0007046875f*0.5f,0); //stretched
            //Abdomen.transform.localEulerAngles = new Vector3(0f, 180f, 0f);
            
            //Chest.transform.localPosition = new Vector3(0,0.001f,0);
            //Chest.transform.localEulerAngles = new Vector3(0f, 180f, 0f);
            
            // ShoulderBoneLeft.transform.localPosition = new Vector3(0,0.001f,0);
            // ShoulderBoneRight.transform.localPosition = new Vector3(0,0.001f,0);
            //ShoulderBoneLeft.transform.eulerAngles = new Vector3(0.01f,0.01f,-90);
            // ShoulderBoneLeft.transform.eulerAngles = getAngleRight(landmarks2D[6],landmarks2D[5]);
            // //ShoulderBoneRight.transform.eulerAngles = new Vector3(0.01f,0.01f,90);
            // ShoulderBoneRight.transform.eulerAngles = getAngleRight(landmarks2D[5],landmarks2D[6]);
            
            ShoulderLeft.transform.localPosition = new Vector3(-shoulderHalfdis*0.0007046875f,0,0);
            //Debug.Log("shoulder left");
            //Debug.Log(ShoulderLeft.transform.position);
            ShoulderRight.transform.localPosition = new Vector3(shoulderHalfdis*0.0007046875f,0,0);
            //Debug.Log("shoulder right");
            //Debug.Log(ShoulderRight.transform.position);
            
            ElbowLeft.transform.localPosition = new Vector3(0,0.5f*shoulder2elbowL*0.0007046875f,0); //0.5 here temporarily
            ElbowRight.transform.localPosition = new Vector3(0,0.5f*shoulder2elbowR*0.0007046875f,0);

            WristLeft.transform.localPosition = new Vector3(0,elbow2wristL*0.0007046875f,0);
            //Debug.Log("wrist left");
            //Debug.Log(ShoulderLeft.transform.position);
            WristRight.transform.localPosition = new Vector3(0,elbow2wristR*0.0007046875f,0);
            /** stop
            **/

            HipLeft.transform.localPosition = new Vector3(0,hipHalfdis*0.0007046875f,0);
            HipRight.transform.localPosition = new Vector3(0,hipHalfdis*0.0007046875f,0);
            //HipLeft.transform.eulerAngles = new Vector3(0f,0f,180f);
            //HipRight.transform.eulerAngles = new Vector3(0f,0f,180f);
            
            //print(getAngleLeft(wristLeftpos,elbowLeftpos));
            ElbowLeft.transform.eulerAngles = -getAngleLeft(landmarks2D[10],landmarks2D[8]);
            ElbowRight.transform.eulerAngles = -getAngleLeft(landmarks2D[9],landmarks2D[7]);
            //print(ElbowLeft.transform.eulerAngles);
            

            ShoulderLeft.transform.eulerAngles = -getAngleLeft(landmarks2D[8],landmarks2D[6]);
            //print(ShoulderLeft.transform.eulerAngles);
            ShoulderRight.transform.eulerAngles = -getAngleLeft(landmarks2D[7],landmarks2D[5]);

            
            Armature.transform.eulerAngles = new Vector3(0f, 0f, 180f); //backwards at 0,180,180 as well as 0,0,180 - there's no change
            //Armature.transform.eulerAngles = new Vector3(0f, 180f, 0f);
        }

        float getDepth(float x1, float y1, float x2, float y2, float length)
        {
            float depth;
            depth = Mathf.Sqrt(Mathf.Pow(length,2) - Mathf.Pow(x1 - x2,2) - Mathf.Pow(y1 - y2,2));
            return depth;
        }
        float getDistance (Vector3 point1, Vector3 point2)
        {
            float distance;
            distance = Mathf.Sqrt( Mathf.Pow(point1.x - point2.x,2) + Mathf.Pow(point1.y - point2.y,2));
            return distance;
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
            //angle.x = -Mathf.Atan(point1.y - point2.y/point1.z - point2.z)*180/Mathf.PI;
            angle.x = 0;
            
            if (point1.x > point2.x)
            {
                angle.z = (Mathf.Atan((point1.y - point2.y)/(point1.x - point2.x))*180/Mathf.PI) + 90f;
                
            }
            else
            {
                angle.z = (Mathf.Atan((point1.y - point2.y)/(point1.x - point2.x))*180/Mathf.PI) - 90f;
                
            }
            //print((point1.x - point2.x)/(point1.y - point2.y));
            //angle.z = -90f;
            //angle.y = -Mathf.Atan(point1.z - point2.z/point1.x - point2.x)*180/Mathf.PI;
            //angle.y = -Mathf.Atan((point1.z - point2.z)/(point1.x - point2.x))*180/Mathf.PI;
            angle.y = 0;
            
            return angle + new Vector3(0f, 180f, 0f);
        }
        Vector3 getAngleRight(Vector2 point1, Vector2 point2)
        {
            Vector3 angle;
            //angle.x = -Mathf.Atan(point1.y - point2.y/point1.z - point2.z)*180/Mathf.PI;
            angle.x = 0;
            
            if (point1.x > point2.x)
            {
                angle.z = (Mathf.Atan((point1.y - point2.y)/(point1.x - point2.x))*180/Mathf.PI) + 90f;
                
            }
            else
            {
                angle.z = (Mathf.Atan((point1.y - point2.y)/(point1.x - point2.x))*180/Mathf.PI) - 90f;
                
            }
            //angle.z = 0;
            //print((point1.x - point2.x)/(point1.y - point2.y));
            //angle.z = -90f;
            //angle.y = -Mathf.Atan(point1.z - point2.z/point1.x - point2.x)*180/Mathf.PI;
            //angle.y = -Mathf.Atan((point1.z - point2.z)/(point1.x - point2.x))*180/Mathf.PI;
            angle.y = 0;
            
            return angle;
        }
        // Vector3 getAngleLeft(Vector3 point1, Vector3 point2)
        // {
        //     Vector3 angle;
        //     //angle.x = -Mathf.Atan(point1.y - point2.y/point1.z - point2.z)*180/Mathf.PI;
        //     angle.x = 0;
            
        //     if (point1.x > point2.x)
        //     {
        //         angle.z = (Mathf.Atan((point1.y - point2.y)/(point1.x - point2.x))*180/Mathf.PI) - 90f;
        //     }
        //     else
        //     {
        //         angle.z = (Mathf.Atan((point1.y - point2.y)/(point1.x - point2.x))*180/Mathf.PI) + 90f;
        //     }
        //     //print((point1.x - point2.x)/(point1.y - point2.y));
        //     //angle.z = -90f;
        //     //angle.y = -Mathf.Atan(point1.z - point2.z/point1.x - point2.x)*180/Mathf.PI;
        //     //angle.y = -Mathf.Atan((point1.z - point2.z)/(point1.x - point2.x))*180/Mathf.PI;
        //     angle.y = 0;
            
        //     return angle;
        // }
        // Vector3 getAngleRight(Vector3 point1, Vector3 point2)
        // {
        //     Vector3 angle;
        //     //angle.x = -Mathf.Atan(point1.y - point2.y/point1.z - point2.z)*180/Mathf.PI;
        //     angle.x = 0;
            
        //     if (point1.x < point2.x)
        //     {
        //         angle.z = (Mathf.Atan((point1.y - point2.y)/(point1.x - point2.x))*180/Mathf.PI) + 90f;
        //     }
        //     else
        //     {
        //         angle.z = (Mathf.Atan((point1.y - point2.y)/(point1.x - point2.x))*180/Mathf.PI) - 90f;
        //     }
        //     //angle.z = 0;
        //     //print((point1.x - point2.x)/(point1.y - point2.y));
        //     //angle.z = -90f;
        //     //angle.y = -Mathf.Atan(point1.z - point2.z/point1.x - point2.x)*180/Mathf.PI;
        //     //angle.y = -Mathf.Atan((point1.z - point2.z)/(point1.x - point2.x))*180/Mathf.PI;
        //     angle.y = 0;
            
        //     return angle;
        // }
    }
}