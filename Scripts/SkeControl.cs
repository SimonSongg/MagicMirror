using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeControl : MonoBehaviour
{
    // Start is called before the first frame update
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

    void Start()
    {
         originalGameObject = GameObject.Find("UnitSkeleton");
         Armature = originalGameObject.transform.GetChild(0).gameObject;
         RootBone = Armature.transform.GetChild(0).gameObject;
         Base = RootBone.transform.GetChild(0).gameObject;
         HipBoneLeft = RootBone.transform.GetChild(1).gameObject;
         HipBoneRight = RootBone.transform.GetChild(2).gameObject;
         HipLeft = HipBoneLeft.transform.GetChild(0).gameObject;
         HipRight = HipBoneRight.transform.GetChild(0).gameObject;
         KneeLeft = HipLeft.transform.GetChild(0).gameObject;
         KneeRight = HipRight.transform.GetChild(0).gameObject;
         Abdomen = Base.transform.GetChild(0).gameObject;
         Chest = Abdomen.transform.GetChild(0).gameObject;
         ShoulderBoneLeft = Abdomen.transform.GetChild(1).gameObject;
         ShoulderBoneRight = Abdomen.transform.GetChild(2).gameObject;
         ShoulderLeft = ShoulderBoneLeft.transform.GetChild(0).gameObject;
         ShoulderRight = ShoulderBoneRight.transform.GetChild(0).gameObject;
         ElbowLeft = ShoulderLeft.transform.GetChild(0).gameObject;
         ElbowRight = ShoulderRight.transform.GetChild(0).gameObject;
         WristLeft = ElbowLeft.transform.GetChild(0).gameObject;
         WristRight = ElbowRight.transform.GetChild(0).gameObject;

    }

    // Update is called once per frame
    void Update()
    {
        //ShoulderLeft.transform.Rotate(0.01f, 0, 0);
        //ShoulderLeft.transform.Translate(0.1f, 0, 0);
        // ShoulderLeft.transform.position = new Vector3(3.77f,12.36f,58.87f);
        // ShoulderRight.transform.position = new Vector3(-5.35f,11.48f,54.68f);

        // ElbowLeft.transform.position = new Vector3(3.39f,2.65f,31.65f);
        // ElbowRight.transform.position = new Vector3(-8.54f,5.08f,60.53f);

        // WristLeft.transform.position = new Vector3(4.37f,-0.41f,32.61f);
        // WristRight.transform.position = new Vector3(-7.66f,-0.72f,41.3f);

        // HipBoneRight.transform.position = new Vector3(0.8f,-1.19f,27.7f);
        // HipBoneLeft.transform.position = new Vector3(-3.27f,-1.42f,36.91f);

        // KneeRight.transform.position = new Vector3(0.19f,-6.77f,27.66f);
        // KneeLeft.transform.position = new Vector3(-8.24f,-13.76f,58.39f);

        Vector3 lefthip = new Vector3(2.41f,0.73f,23.26f);
        Vector3 righthip = new Vector3(-0.76f,0.93f,23.49f);
        //print(HipBoneLeft.transform.position);
        Vector3 ShoulderLeftpos = new Vector3(4.79f,9.96f,31.62f);
        Vector3 ShoulderRightpos = new Vector3(-1.38f,8.1f,25.72f);

        Vector3 elbowLeftpos = new Vector3(7.5f,5.94f,26.13f);
        Vector3 elbowRightpos = new Vector3(-4.51f,6.01f,26.87f);

        Vector3 wristLeftpos = new Vector3(11.63f,7.54f,29.68f);
        Vector3 wristRightpos = new Vector3(-8.24f,5.72f,26.09f);

        Vector3 kneeLeftpos = new Vector3(1.86f,-4.74f,24.20f);
        Vector3 kneeRightpos = new Vector3(-0.98f,-4.70f,24.20f);

        float mainDis = getDistance((lefthip+righthip)/2,(ShoulderLeftpos+ShoulderRightpos)/2);
        print(mainDis);
        RootBone.transform.position = (lefthip+righthip)/2;
        //print(RootBone.transform.position);
        Abdomen.transform.localPosition = new Vector3(0,(mainDis)*0.01f,0); // Y axis is the half of the link length between the hip and the shoulder


        Chest.transform.localPosition = new Vector3(0,0.01f,0);

        ShoulderBoneLeft.transform.localPosition = new Vector3(0,0.01f,0);
        ShoulderBoneRight.transform.localPosition = new Vector3(0,0.01f,0);
        ShoulderBoneLeft.transform.eulerAngles = new Vector3(0.01f,0.01f,-90);
        ShoulderBoneRight.transform.eulerAngles = new Vector3(0.01f,0.01f,90);

        ShoulderLeft.transform.localPosition = new Vector3(0,3.1f*0.01f,0);
        ShoulderRight.transform.localPosition = new Vector3(0,3.1f*0.01f,0);

        ElbowLeft.transform.localPosition = new Vector3(0,4.52f*0.01f,0);
        ElbowRight.transform.localPosition = new Vector3(0,4.50f*0.01f,0);

        WristLeft.transform.localPosition = new Vector3(0,3f*0.01f,0);
        WristRight.transform.localPosition = new Vector3(0,3f*0.01f,0);

        

        ElbowLeft.transform.eulerAngles = getAngleLeft(wristLeftpos,elbowLeftpos);
        ElbowRight.transform.eulerAngles = getAngleRight(wristRightpos,elbowRightpos);
        //print(ElbowLeft.transform.eulerAngles);
        

        ShoulderLeft.transform.eulerAngles = getAngleLeft(elbowLeftpos,ShoulderLeftpos);
        //print(ShoulderLeft.transform.eulerAngles);
        ShoulderRight.transform.eulerAngles = getAngleRight(elbowRightpos,ShoulderRightpos);

        // ShoulderLeft.transform.position = new Vector3(-0.28f,10.42f,48.54f);
        // ShoulderRight.transform.position = new Vector3(-7.71f,10.14f,47.25f);

        

        

        

        // KneeLeft.transform.position = new Vector3(-0.99f,-6.97f,27.55f);
        // KneeRight.transform.position = new Vector3(-6.09f,-7.94f,33.69f);

        //Chest.transform.position = new Vector3((ShoulderLeft.transform.position.x+ShoulderRight.transform.position.x)/2,(ShoulderLeft.transform.position.y+ShoulderRight.transform.position.y)/2,(ShoulderLeft.transform.position.z+ShoulderRight.transform.position.z)/2);
        //Chest.transform.position = new Vector3((ShoulderLeft.transform.position.x+ShoulderRight.transform.position.x)/2,(ShoulderLeft.transform.position.y+ShoulderRight.transform.position.y)/2,RootBone.transform.position.z);
        // ShoulderBoneLeft.transform.position = Chest.transform.position;
        // ShoulderBoneRight.transform.position = Chest.transform.position;

        
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
    Vector3 getAngleLeft(Vector3 point1, Vector3 point2)
    {
        Vector3 angle;
        //angle.x = -Mathf.Atan(point1.y - point2.y/point1.z - point2.z)*180/Mathf.PI;
        angle.x = 0;
        
        angle.z = (Mathf.Atan((point1.x - point2.x)/(point1.y - point2.y))*180/Mathf.PI) - 90f;
        //print((point1.x - point2.x)/(point1.y - point2.y));
        //angle.z = -90f;
        //angle.y = -Mathf.Atan(point1.z - point2.z/point1.x - point2.x)*180/Mathf.PI;
        angle.y = -Mathf.Atan((point1.z - point2.z)/(point1.x - point2.x))*180/Mathf.PI;
        //angle.y = 0;
        
        return angle;
    }
    Vector3 getAngleRight(Vector3 point1, Vector3 point2)
    {
        Vector3 angle;
        //angle.x = -Mathf.Atan(point1.y - point2.y/point1.z - point2.z)*180/Mathf.PI;
        angle.x = 0;
        
        angle.z = (Mathf.Atan((point1.x - point2.x)/(point1.y - point2.y))*180/Mathf.PI) + 90f;
        //print((point1.x - point2.x)/(point1.y - point2.y));
        //angle.z = -90f;
        //angle.y = -Mathf.Atan(point1.z - point2.z/point1.x - point2.x)*180/Mathf.PI;
        angle.y = -Mathf.Atan((point1.z - point2.z)/(point1.x - point2.x))*180/Mathf.PI;
        //angle.y = 0;
        
        return angle;
    }
}
