using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControler : MonoBehaviour
{
    Animator anim;          //애니메이터 컴포넌트
    Rigidbody rigid;        //리지드바디 컴포넌트
  
    public Transform cameraArm;    //메인카메라 트랜스폼     
    public Transform slimeBody;    //슬라임 트랜스폼 
    public Transform skillPos;
    public GameObject skillElement;

    GameObject nearObj;     //가까이 있는 오브젝트를 저장할 변수
    GameObject instantSkill;
    

    public float spd=5f;    //캐릭터 걷기 이동속도
    public float jumppw=10f;//점프 파워
    public int damage=20;
    public int elementCnt=0;    //엘리먼트 강화 숫자
    public int elementIndex=-2; //불:0, 얼음:1, 물:2, 돌:3, 풀:4, 빛:5의 인덱스를 가짐 -2는 초기화
    public int myElement=-1;    //내가 가진 원소의 Index, -1은 초기화
    public int myHealth=100;    

    float hAxis;            //키보드 a,d 또는 방향키 양옆 입력 받을 변수
    float vAxis;            //키보드 w,s 또는 방향키 앞뒤 입력 받을 변수
    float xMouse;           //마우스 x축 받을 변수
    float yMouse;           //마우스 y축 받을 변수

    Vector3 moveVec;        //이동 값 관련 변수
    Vector3 jumpVec;        //점프 중 방향 전환 불가
    Vector3 lookForward;    //카메라 회전 관리 
    Vector3 slimeForward;
    Vector3 slimeRight;
    

    bool runflag;
    bool jumpflag;
    bool Altflag;
    bool isJump;
    bool isDodge;
    bool isAttack;
    bool isSkill;
    bool Eflag;
    bool attackFlag;
    bool skillFlag;
    


    void Awake()
    {
        rigid=GetComponent<Rigidbody>();          //리지드바디 컴포넌트 호출
        anim=slimeBody.GetComponent<Animator>();  //애니메이터 컴포넌트 호출
    }

    // Update is called once per frame
    void Update()
    {   
        GetInput();
        LookAround();
        Move();
        Run();
        Jump();
        Attack();
        Skill();
        Interaction();
    }
    
    

    void GetInput()
    {
        hAxis=Input.GetAxisRaw("Horizontal"); 
        vAxis=Input.GetAxisRaw("Vertical");
        xMouse=Input.GetAxis("Mouse X");
        yMouse=Input.GetAxis("Mouse Y");
        runflag=Input.GetKey(KeyCode.LeftShift);
        jumpflag=Input.GetButtonDown("Jump");
        Altflag=Input.GetKey(KeyCode.LeftAlt);
        Eflag=Input.GetKeyDown(KeyCode.E);
        attackFlag=Input.GetMouseButtonDown(1);
        skillFlag=Input.GetMouseButtonDown(0);
        //입력받는 함수
    }

    void LookAround()
    {   
        Vector2 mouseDelta = new Vector2(xMouse,yMouse);        //이동 전후의 마우스 값 차이
        Vector3 camAngle = cameraArm.rotation.eulerAngles;      //카메라 회전을 오일러 값으로 변환
        float xLimit = camAngle.x - mouseDelta.y;               //71번 라인 x좌표에 들어가기 전에 조건문을 위해 변수 선언 후 미리 계산
        if(xLimit<180f) xLimit = Mathf.Clamp(xLimit, -1f,70f);  //하늘을 쳐다보는 경우 카메라앵글 제한
        else xLimit = Mathf.Clamp(xLimit,335f,361f);            //땅을 쳐다보는 경우 카메라앵글 제한
        cameraArm.rotation = Quaternion.Euler(xLimit , camAngle.y + mouseDelta.x,camAngle.z);   //camAngle에 mouseDelta를 더함(영향)
        
        lookForward = new Vector3(cameraArm.forward.x, 0f, cameraArm.forward.z);    //카메라 앵글에 대한 벡터
        slimeForward = new Vector3(slimeBody.forward.x, 0f, slimeBody.forward.z).normalized;    //슬라임이 보고 있는 방향에 대한 벡터을 0~1 사이 값으로 normalized(앞뒤)
        slimeRight = new Vector3(slimeBody.right.x, 0f, slimeBody.right.z).normalized;          //좌우
        
        if(!Altflag)
        {
            slimeBody.forward = lookForward;    //Alt키가 눌려 있지 않은 경우 몸 방향 = 카메라가 보는 방향
            //마우스 이동에 따라 카메라가 보는 방향이 변하고 카메라 보는 방향 따라 몸 방향이 따라가는 구조
        }
    }

    void Move()
    {
        if(hAxis==0&&vAxis==0) anim.SetBool("isWalk",false);
            //이동 입력이 없을 때 
        else {
            anim.SetBool("isWalk",true);
            moveVec = slimeForward * vAxis + slimeRight * hAxis;   
            if(isJump) moveVec=jumpVec;     // 점프했을 때 방향 전환 불가능
            transform.position += moveVec * spd * Time.deltaTime;   //Time.deltaTime -> 사용자 프레임에 상관없이 동일하게 출력
            rigid.MovePosition (transform.position);
        }     
    }
            
    void Run() 
    {
        if(runflag&&Input.GetKey(KeyCode.W)){  //앞으로만 달리기 가능
            anim.SetBool("isRun", true);
            spd=10f;
        }
        else {
            anim.SetBool("isRun",false); 
            spd=5f;
        }
    }

    void Jump()
    {
        if(jumpflag&&!isJump)
        {   
            jumpVec=moveVec;
            rigid.AddForce(Vector3.up*jumppw,ForceMode.Impulse);    //Vector3기준 up(위)방향으로 AddForce로 점프, 점프모드 Impulse로 즉발
            anim.SetBool("isJump",true);    //점프 애니메이션 실행을 위한 flag
            anim.SetTrigger("doJump");      //점프 애니메이션 실행을 위한 트리거 실행
            isJump=true;                    //점프 애니메이션 실행이 아닌 코드 내부 점프 여부 확인 flag
        }
    }

    void Attack()
    {
        if(attackFlag&&!isAttack) {
            isAttack=true;
            rigid.AddForce(slimeForward*30f,ForceMode.Impulse); //슬라임이 보는 방향으로 박치기
            StartCoroutine(WaitforAttack());
            //Attack 함수와 WaitforAttack() 함수는 메인 루틴 -> 서브 루틴 관계가 아닌 동시에 실행
        }
    }

    IEnumerator WaitforAttack() {
        yield return new WaitForSeconds(2.0f);  //코루틴 딜레이를 통해 플래그 수정
        isAttack=false;
    }

    void Skill()
    {
        if(skillFlag && !isAttack && myElement<0 && !isSkill){
            isSkill=true;
            StartCoroutine(CreateSkill());
        }
    }

    IEnumerator CreateSkill()
    {
        instantSkill = Instantiate(skillElement, skillPos.position, skillPos.rotation);
        Rigidbody skillRigid = instantSkill.GetComponent<Rigidbody>();
        skillRigid.velocity = slimeForward * 20;
        skillRigid.AddTorque(Vector3.up * 10, ForceMode.Impulse);
        yield return new WaitForSeconds(0.7f);
        isSkill=false;
    }

    void Heal(){
        if(myHealth<=70) myHealth+=30;
        else myHealth = myHealth+(100-myHealth);
    }

    void Interaction()
    {   
        if(Eflag && nearObj != null && !isJump && nearObj.tag == "Tag_Element")
        {   
            Elements el = nearObj.GetComponent<Elements>(); //nearObj의 컴포넌트에서 스크립트내에 Type이 Elements의 객체
            elementIndex=el.value;
            if(myElement==-1){  //원소를 가지고 있지 않은 경우
                elementCnt++;
                myElement=elementIndex;
            }
            
            else if(myElement==elementIndex) {  //내가 먹은 원소가 내가 가지고 있던 원소와 같은 경우
                if(elementCnt<5){   //원소 강화 최대치 +5
                    damage+=10;
                    elementCnt++;
                }
            }
            else {  //내가 가지고 있던 원소랑 내가 먹은 원소가 다른경우
                if(elementCnt>=2) Heal();   //원소 강화 +2 이상일 경우에만 힐 -> 피가 없을 경우 계속 다른 종류 원소 먹는 경우 방지(어느정도 리스크 감수 해야 힐가능)
                damage=20;
                elementCnt=1;
                myElement=elementIndex;
            }
            Destroy(nearObj); //상호작용 시 파괴   
        }
    }
    
    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag=="Floor") { //태그가 "Floor"인 오브젝트랑 충돌 시 true
            anim.SetBool("isJump",false);
            isJump = false;
        }
    }

    void OnTriggerStay(Collider other) //트리거(콜라이더 범위)에 닿여있을 때 실행하는 함수
    {
        if(other.tag == "Tag_Element") nearObj = other.gameObject; //원소 트리거 발생 시 nearObj에 부딛힌 오브젝트를 저장
        //Sphere Collider 에 isTrigger 체크 시에 충돌체가 아닌 트리거가 된다.
    }

    void OnTriggerExit(Collider other)
    {
        if(other.tag == "Tag_Element") nearObj = null;
        //콜라이더 범위 탈출 시 nearObj 초기화
    }
    
}
