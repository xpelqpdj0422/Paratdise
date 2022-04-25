using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �ۼ��� : ������
/// �����ۼ��� : 2022/04/20
/// ���������� : 
/// ���� : 
/// 
/// �÷��̾��� ���� ���� �ӽŵ��� �����ϴ� �Ŵ���Ŭ����
/// </summary>
public class PlayerStateMachineManager : MonoBehaviour
{
    public static PlayerStateMachineManager instance;

    public PlayerState state;
    public float moveSpeed;
    public Vector2 move;
    Vector2 _direction;
    public Vector2 direction
    {
        set
        {
            if (state != PlayerState.Movement) return;

            Vector2 overlapSize = Vector2.zero;
            if (value != _direction)
            {
                if (value == Vector2.up)
                {
                    modelManager.LookBack();
                    overlapSize = new Vector2(col.size.x, 0.01f);
                }   
                else if (value == Vector2.down)
                {
                    modelManager.LookFront();
                    overlapSize = new Vector2(col.size.x, 0.01f);
                }   
                else if (value == Vector2.left)
                {
                    modelManager.LookLeft();
                    overlapSize = new Vector2(0.01f, col.size.x);
                }   
                else if (value == Vector2.right)
                {
                    modelManager.LookRight();
                    overlapSize = new Vector2(0.01f, col.size.x);
                }   

                _direction = value;
            }

            Collider2D mapTile = Physics2D.OverlapBox(rb.position + _direction * (col.size.y / 2), overlapSize, 0, digTargetLayer);

            if (mapTile != null)
                Debug.Log(mapTile);

            if (mapTile != null &&
                mapTile.tag == "Destroyable")
            {
                ChangeState(PlayerState.Dig);
                move = Vector2.zero;
            }
            else
            {   
                move = value;
            }

            modelManager.SetFloat("h", move.x);
            modelManager.SetFloat("v", move.y);
        }
        get { return _direction; }
    }
    [SerializeField] private LayerMask digTargetLayer;

    Rigidbody2D rb;
    Transform tr;
    CapsuleCollider2D col;

    PlayerStateMachine[] machines;
    PlayerStateMachine currentMachine;
    PlayerModelManager modelManager;


    //============================================================================
    //************************* Public Methods ***********************************
    //============================================================================

    public void ChangeState(PlayerState newState)
    {
        if (state == newState) return;

        foreach (var sub in machines)
        {
            if (sub.playerState == newState &&
                sub.IsExecuteOK())
            {
                move = Vector2.zero;
                modelManager.SetFloat("h", move.x);
                modelManager.SetFloat("v", move.y);
                currentMachine.ForceStop();
                currentMachine = sub;
                currentMachine.Execute();
                state = newState;
            }
        }
    }


    //============================================================================
    //************************* Private Methods **********************************
    //============================================================================

    private void Awake()
    {
        instance = this;
        machines = GetComponents<PlayerStateMachine>();
        modelManager = GetComponent<PlayerModelManager>();
        rb = GetComponent<Rigidbody2D>();
        tr = GetComponent<Transform>();
        col = GetComponent<CapsuleCollider2D>();
        currentMachine = GetMachine(PlayerState.Movement);
        PlayStateManager.instance.OnPlayStateChanged += OnPlayStateChanged;
    }

    private void OnDestroy()
    {
        PlayStateManager.instance.OnPlayStateChanged -= OnPlayStateChanged;
    }

    private void OnPlayStateChanged(PlayState newPlayState)
    {
        enabled = newPlayState == PlayState.Play;
    }

    private void Start()
    {
        direction = Vector2.up;
    }

    private void Update()
    {
        

        UpdateState();
    }

    private void FixedUpdate()
    {        
        tr.Translate(move * moveSpeed * Time.fixedDeltaTime);
    }

    private void UpdateState()
    {
        ChangeState(currentMachine.Workflow());
    }

    private PlayerStateMachine GetMachine(PlayerState targetState)
    {
        foreach (var sub in machines)
        {
            if (sub.playerState == targetState)
                return sub;
        }
        return null;
    }

    private void OnDrawGizmos()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<CapsuleCollider2D>();
        Vector2 overlapSize = Vector2.zero;
        if (_direction == Vector2.up)
            overlapSize = new Vector2(col.size.x, 0.01f);
        else if (_direction == Vector2.down)
            overlapSize = new Vector2(col.size.x, 0.01f);
        else if (_direction == Vector2.left)
            overlapSize = new Vector2(0.01f, col.size.x);
        else if (_direction == Vector2.right)
            overlapSize = new Vector2(0.01f, col.size.x);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(rb.position + _direction * (col.size.y / 2), overlapSize);


    }
}

public enum PlayerState
{
    Idle,
    Movement,
    Dig,
    Attack,
    Hurt,
    Die,
}