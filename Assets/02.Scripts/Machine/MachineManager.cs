using Unity.VisualScripting;
using UnityEditor.VersionControl;
using UnityEngine;
using Utils.EnumTypes;

public class MachineManager : MonoBehaviour
{
    private static MachineManager instance;
    public static MachineManager Instance {  get { return instance; } }

    private RaycastHit2D hit;
    public BasketController basket;
    public MachineController machine;

    public int clickCount = 0;

    private void Awake()
    {
        if (instance != null)
            Destroy(instance);
        else
            instance = this;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 _pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            hit = Physics2D.Raycast(_pos, Vector2.zero, 0.0f);

            if (hit.collider != null)
            {
                if (hit.collider.CompareTag("Basket"))
                    OnBasketSelect();
                else if (hit.collider.CompareTag("Machine"))
                    OnMachineSelect();
            }
        }
    }

    // 기계 선택
    public void OnMachineSelect()
    {
        MachineController _machine = hit.collider?.GetComponent<MachineController>();

        if(basket == null)
        {
            if (_machine.machineState == MachineState.Complete)
            {
                // 빨래 완료 된 기계에서 빨랫감 꺼내기
                machine = _machine;
                OnColorHandler(machine.gameObject, Color.red);
                basket = machine.currentBasket;

                if(_machine.machineType == MachineType.IroningBoard)
                {
                    basket.OnComplete();
                    OnColorHandler(machine.gameObject, Color.white);
                    machine.Init();
                    machine = null;
                    basket = null;
                }
            }
        }
        else if(basket != null)
        {
            if(_machine.machineState == MachineState.Idle && basket.laundryState == _machine.laundryState)
            {
                // 비어 있는 기계에 빨래 넣기
                if (machine != null)
                {
                    _machine.currentBasket = basket;
                    _machine.SetTime(basket.laundryCount);
                    _machine.machineState = MachineState.Working;

                    OnColorHandler(machine.gameObject, Color.white);
                    machine.Init();
                    machine = null;

                    basket.OnNextStep();
                    basket.gameObject.SetActive(false);
                    basket = null;
                }
                else
                {
                    _machine.currentBasket = basket;
                    _machine.SetTime(basket.laundryCount);
                    _machine.machineState = MachineState.Working;

                    OnColorHandler(basket.gameObject, Color.white);
                    machine = null;

                    basket.OnNextStep();
                    basket.gameObject.SetActive(false);
                    basket = null;
                }
            }
            else
            {
                if (machine != null)
                {
                    OnColorHandler(machine.gameObject, Color.white);
                    machine = null;
                }
                else
                {
                    OnColorHandler(basket.gameObject, Color.white);
                    basket = null;
                }
            }
        }
    }

    // 빨랫 바구니 선택
    public void OnBasketSelect()
    {
        if (machine != null)
        {
            OnColorHandler(machine.gameObject, Color.white);
            machine = null;
        }

        if (basket == null)
        {
            basket = hit.collider?.GetComponent<BasketController>();
            OnColorHandler(basket.gameObject, Color.red);
        }
        else
        {
            OnColorHandler(basket.gameObject, Color.white);
            basket = hit.collider?.GetComponent<BasketController>();
            OnColorHandler(basket.gameObject, Color.red);
        }
    }

    // 오브젝트 색상 변경
    public void OnColorHandler(GameObject _object, Color _color)
    {
        _object.GetComponent<SpriteRenderer>().color = _color;
    }
}