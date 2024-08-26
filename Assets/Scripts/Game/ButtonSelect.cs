using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class MyEventTrigger : MonoBehaviour,
    //IEventSystemHandler, 
    IPointerEnterHandler,
    IPointerExitHandler,
    //IPointerDownHandler, 
    //IPointerUpHandler, 
    IPointerClickHandler,
    //IBeginDragHandler, 
    //IInitializePotentialDragHandler, 
    //IDragHandler, 
    //IEndDragHandler, 
    //IDropHandler, 
    //IScrollHandler, 
    //IUpdateSelectedHandler, 
    ISelectHandler
    //IDeselectHandler, 
    //IMoveHandler, 
    //ISubmitHandler, 
    //ICancelHandler
{
    [Serializable] 
    public class EditEvent
    {
        public bool PointerEnter;
        public bool PointerExit;
        //public bool PointerDown;
        //public bool PointerUp;
        public bool PointerClick;
        //public bool BeginDrag;
        //public bool InitializePotentialDrag;
        //public bool Drag;
        //public bool EndDrag;
        //public bool Drop;
        //public bool Scroll;
        //public bool UpdateSelected;
        public bool Select;
    } 

    public EditEvent editEvent;

    [Serializable]
    public class Entry
    {
        public EventTriggerType triggerData = EventTriggerType.PointerClick;

        public TriggerEvent triggerEvent = new TriggerEvent();
    }

    [Serializable]
    public class TriggerEvent : UnityEvent<BaseEventData>
    {

    }
    [SerializeField]
    [FormerlySerializedAs("delegates")] 
    private List<Entry> myDelegate;
    public List<EventTrigger.Entry> delegates;

    public EventTrigger test;

    public List<Entry> trigger
    {
        get
        {
            if (myDelegate == null)
            {
                myDelegate = new List<Entry>();
            }

            return myDelegate;
        }
        set
        {
            myDelegate = value;
        }
    }

    private void ExecuteEvent(EventTriggerType type, BaseEventData eventData)
    {
        int index = 0;

        for (int i = trigger.Count; index < i; ++i)
        {
            Entry entry = trigger[i];

            if (entry.triggerData == type && entry.triggerEvent != null)
            {
                entry.triggerEvent.Invoke(eventData);
            }
        }
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        ExecuteEvent(EventTriggerType.PointerClick, eventData);
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        ExecuteEvent(EventTriggerType.PointerEnter, eventData);
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        ExecuteEvent(EventTriggerType.PointerExit, eventData);
    }

    public virtual void OnSelect(BaseEventData eventData)
    {
        ExecuteEvent(EventTriggerType.Select, eventData);
    }
}


public class ButtonSelect : MyEventTrigger
{
    private enum SelectType
    {
        Normal,
        DoubleClick,
    }

    [SerializeField] private GameObject selected;
    [SerializeField] private bool isFirstButton;

    [SerializeField] private SoundManager.Effect effect;

    void Awake()
    {
        test = GetComponent<EventTrigger>();
        //AddEventTrigger(entry, EventTriggerType.PointerClick, entry.callback);
    }


    public void OnPointerEnter()
    {
        selected.SetActive(true);


        if (SoundManager.Instance != null) SoundManager.Instance.PlaySound(effect.ToString());
    }


    //Event

    

    


    //Event

    public void DoubleClick()
    {
        if (SeletedCancelCorotuine != null)
        {
            SeletedCancelCorotuine = null;
        }

        SeletedCancelCorotuine = StartCoroutine(SelectedCancel());

        if (select)
        {
            StopCoroutine(SeletedCancelCorotuine);

            
        }
        
        select = true;
    }

    private void AddEventTrigger(EventTrigger trigger, EventTriggerType type,  UnityEngine.Events.UnityAction<BaseEventData> callback)
    {
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = type;
        entry.callback.AddListener(callback);
        trigger.triggers.Add(entry);
    }

    [SerializeField] private float selectCancelDelay;
    Coroutine SeletedCancelCorotuine;
    private bool select;

    IEnumerator SelectedCancel()
    {
        yield return new WaitForSeconds(selectCancelDelay);

        select = false;
    }
}
