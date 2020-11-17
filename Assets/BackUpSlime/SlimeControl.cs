using System.Collections;
using UnityEngine;

public class SlimeControl : MonoBehaviour
{
    Animator anim; //애니메이터 컴포넌트
    public float spd=15f; //캐릭터 걷기 이동속도
    public float runspd=40f; //캐릭터 달리기 이동속도
    public float rotnspd=15f; //캐릭터 회전속도
    Rigidbody rigi; //리지드바디 컴포넌트
    float h;
    float v;
    float r; // 달리기
    Vector3 moves; //이동 값 관련 변수
    void Start()
    {
        rigi=GetComponent<Rigidbody>(); //리지드바디 컴포넌트 호출
        anim=GetComponent<Animator>();  //애니메이터 컴포넌트 호출
    }

    // Update is called once per frame
    void Update()
    {
        rot();
        ani();
        h=Input.GetAxisRaw("Horizontal"); 
        v=Input.GetAxisRaw("Vertical");
        Move(h,v);
        if(Input.GetKey(KeyCode.LeftShift)){
            anim.SetBool("MIRun", true);    //왼쪽 시프트 입력 받을 때만 애니메이션 출력
            run(h,v);                       //왼쪽 시프트 입력 받을 때만 달리기 함수 호출
        }
        else anim.SetBool("MIRun",false);   //입력이 없을 시 달리기 애니메이션 호출 변수 false
    }

    void Move(float h, float v)
    {
        moves.Set(h,0,v);
        moves=(moves.normalized*spd*Time.deltaTime);    //Time.deltaTime -> 사용자 프레임에 상관없이 동일하게 출력
        rigi.MovePosition (transform.position+moves);   //이동시기는 함수
        //부드럽게 이동 시켜줌
    }

    void rot(){
        if(h==0&&v==0) return; //입력 없을 때 회전 방지
        Quaternion rotn=Quaternion.LookRotation(moves);
        rigi.rotation = Quaternion.Slerp(rigi.rotation, rotn, rotnspd*Time.deltaTime);
        //쿼터니언은 회전값과 관련된 함수이다. 첫줄은 벡터값에 따른 방향을 바라보게 해주고 
        //두번째 줄은 그방향으로 부드럽게 이동하게 하는 코드이다.(Slerp)
    }

    void ani(){
        if(h==0&&v==0)
        {
            anim.SetBool("MIWalk",false);
            //애니메이터 컨트롤러는 이름.Set~~()이런 형식으로 값을 바꿔준다.
        }
        else anim.SetBool("MIWalk",true);
    }

    void run(float h,float v) 
    {
        moves.Set(h,0,v);
        moves=(moves.normalized*runspd*Time.deltaTime);
        rigi.MovePosition (transform.position+moves);
    }
}

