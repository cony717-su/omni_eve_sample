using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[RequireComponent(typeof(ScrollRect))]
public class ScrollViewController<T> : ViewController		// ViewController 클래스를 상속
{
    protected List<T> listData = new List<T>();			// 리스트 항목의 데이터를 저장
	[SerializeField] private RectOffset padding;			// 스크롤할 내용의 패딩
	[SerializeField] private float spacingHeight = 4.0f;	// 각 셀의 간격
    // Scroll Rect 컴포넌트를 캐시한다
	private ScrollRect _cachedScrollRect;
	public ScrollRect CachedScrollRect
	{
		get {
			if(_cachedScrollRect == null) { 
				_cachedScrollRect = GetComponent<ScrollRect>(); }
			return _cachedScrollRect;
		}
	}

    // 인스턴스를 로드할 때 호출된다
	protected virtual void Awake()
	{
	}

    // 리스트 항목에 대응하는 셀의 높이를 반환하는 메서드
	protected virtual Vector2 GetSlotSize()
	{
        // 실제 값을 반환하는 처리는 상속한 클래스에서 구현한다
        return new Vector2(0.0f, 0.0f);
	}

	protected virtual int GetContraintSlotCount()
	{
		return 0;
	}

    // 스크롤할 내용 전체의 높이를 갱신하는 메서드
	protected void UpdateContentSize()
	{
		int rowCount = GetContraintSlotCount();
		int heightSlotCount = GetFullSlotCount(listData.Count);
		Vector2 slotSize = GetSlotSize();
		float slotHeight = slotSize.y;
		
		// 스크롤할 내용 전체의 높이를 계산한다
		float contentHeight = 0.0f;
		for(int i = 0; i < heightSlotCount; i++)
		{
			contentHeight += slotHeight;
			if (i > 0)
			{
				contentHeight += spacingHeight;
			}
		}

        // 스크롤할 내용의 높이를 설정한다
		Vector2 sizeDelta = CachedScrollRect.content.sizeDelta;
		sizeDelta.y = padding.top + contentHeight + padding.bottom;
		CachedScrollRect.content.sizeDelta = sizeDelta;
	}

#region 셀을 작성하는 메소드와 셀의 내용을 갱신하는 메소드의 구현
	[SerializeField] private GameObject slotBase;	// 복사 원본 셀
	private LinkedList<ScrollViewSlot<T>> slotList =
        new LinkedList<ScrollViewSlot<T>>();			// 셀을 저장

    // 인스턴스를 로드할 때 Awake 메서드 다음에 호출된다
	protected virtual void Start()
	{
        // 복사 원본 셀은 비활성화해둔다
        slotBase.SetActive(false);

#region 셀을 재이용하는 처리를 구현
		// Scroll Rect 컴포넌트의 On Value Changed 이벤트의 이벤트 리스너를 설정한다
		CachedScrollRect.onValueChanged.AddListener(OnScrollPosChanged);
#endregion
	}

    // 셀을 생성하는 메서드
	private ScrollViewSlot<T> CreateSlotForIndex(int index)
	{
        // 복사 원본 셀을 이용해 새로운 셀을 생성한다
		GameObject obj = Instantiate(slotBase) as GameObject;
		obj.SetActive(true);
		ScrollViewSlot<T> slot = obj.GetComponent<ScrollViewSlot<T>>();

        // 부모 요소를 바꾸면 스케일이나 크기를 잃어버리므로 변수에 저장해둔다
		Vector3 scale = slot.transform.localScale;
		Vector2 sizeDelta = slot.CachedRectTransform.sizeDelta;
		Vector2 offsetMin = slot.CachedRectTransform.offsetMin;
		Vector2 offsetMax = slot.CachedRectTransform.offsetMax;

		slot.transform.SetParent(slotBase.transform.parent);

        // 셀의 스케일과 크기를 설정한다
        slot.transform.localScale = scale;
        slot.CachedRectTransform.sizeDelta = sizeDelta;
        slot.CachedRectTransform.offsetMin = offsetMin;
        slot.CachedRectTransform.offsetMax = offsetMax;

        // 지정된 인덱스가 붙은 리스트 항목에 대응하는 셀로 내용을 갱신한다
		UpdateSlotForIndex(slot, index);

		slotList.AddLast(slot);

		return slot;
	}

    // 셀의 내용을 갱신하는 메서드
	private void UpdateSlotForIndex(ScrollViewSlot<T> slot, int index)
	{
        // 셀에 대응하는 리스트 항목의 인덱스를 설정한다
        slot.DataIndex = index;

		if(slot.DataIndex >= 0 && slot.DataIndex <= listData.Count-1)
		{
            // 셀에 대응하는 리스트 항목이 있다면 셀을 활성화해서 내용을 갱신하고 높이를 설정한다
            slot.gameObject.SetActive(true);
            var i = slot.DataIndex;
            var data = listData[i];
            slot.UpdateContent(data);
            Vector2 slotSize = GetSlotSize();
            slot.Height = slotSize.y;
            slot.Width = slotSize.x;
		}
		else
		{
            // 셀에 대응하는 리스트 항목이 없다면 셀을 비활성화시켜 표시되지 않게 한다
            slot.gameObject.SetActive(false);
		}
	}
#endregion

#region visibleRect의 정의와 visibleRect를 갱신하는 메서드 구현
    private Rect _visibleRect;								// 리스트 항목을 셀의 형태로 표시하는 범위를 나타내는 사각형
	[SerializeField] private RectOffset visibleRectPadding;	// visibleRect의 패딩

    // visibleRect을 갱신하기 위한 메서드
	private void UpdateVisibleRect()
	{
        // visibleRect의 위치는 스크롤할 내용의 기준으로부터 상대적인 위치다
        _visibleRect.x = CachedScrollRect.content.anchoredPosition.x + visibleRectPadding.left;
        _visibleRect.y = -CachedScrollRect.content.anchoredPosition.y + visibleRectPadding.top;

        // visibleRect의 크기는 스크롤 뷰의 크기 + 패딩グ
        _visibleRect.width = CachedRectTransform.rect.width + 
                             visibleRectPadding.left + visibleRectPadding.right;
        _visibleRect.height = CachedRectTransform.rect.height + 
                              visibleRectPadding.top + visibleRectPadding.bottom;
	}
#endregion

#region 테이블 뷰의 표시 내용을 갱신하는 처리의 구현
	protected void UpdateContents()
	{
        UpdateContentSize();	// 스크롤할 내용의 크기를 갱신한다
        UpdateVisibleRect();	// visibleRect를 갱신한다

		if(slotList.Count < 1)
		{
            // 셀이 하나도 없을 때는 visibleRect의 범위에 들어가는 첫 번째 리스트 항목을 찾아서
            // 그에 대응하는 셀을 작성한다
			Vector2 slotTop = new Vector2(0.0f, -padding.top);
			Vector2 slotSize = GetSlotSize();
			
			Vector2 slotBottom = slotTop + new Vector2(0.0f, -slotSize.y);
			if((slotTop.y <= _visibleRect.y && 
			    slotTop.y >= _visibleRect.y - _visibleRect.height) || 
			   (slotBottom.y <= _visibleRect.y && 
			    slotBottom.y >= _visibleRect.y - _visibleRect.height))
			{
				ScrollViewSlot<T> slot = CreateSlotForIndex(0);
				slot.Top = slotTop;
				slot.Bottom = slotTop + new Vector2(0.0f, -slotSize.y);
				FillVisibleRectWithSlots();
			}
		}
		else
		{
            // 이미 셀이 있을 때는 첫 번째 셀부터 순서대로 대응하는 리스트 항목의
            // 인덱스를 다시 설정하고 위치와 내용을 갱신한다
			LinkedListNode<ScrollViewSlot<T>> node = slotList.First;
			UpdateSlotForIndex(node.Value, node.Value.DataIndex);
			node = node.Next;
			
			while(node != null)
			{
				UpdateSlotForIndex(node.Value, node.Previous.Value.DataIndex + 1);
				node.Value.Top = 
					node.Previous.Value.Bottom + new Vector2(0.0f, -spacingHeight);
				node = node.Next;
			}
		}
		
		// visibleRect의 범위에 빈 곳이 있으면 셀을 작성한다
		//FillVisibleRectWithSlots();

		//var a = slotList;
	}

	private int GetFullSlotCount(int slotCount)
	{
		int constraintSlotCount = GetContraintSlotCount();
		int height = slotCount / constraintSlotCount;

		if (slotCount % constraintSlotCount != 0)
		{
			height += 1;
		}
		
		return height * constraintSlotCount;
	}

    // visibleRect 범위에 표시될 만큼의 셀을 작성하는 메서드
	private void FillVisibleRectWithSlots()
	{
		// 셀이 없다면 아무 일도 하지 않는다
		if(slotList.Count < 1)
		{
			return;
		}

        // 표시된 마지막 셀에 대응하는 리스트 항목의 다음 리스트 항목이 있고
        // 또한 그 셀이 visibleRect의 범위에 들어온다면 대응하는 셀을 작성한다
        ScrollViewSlot<T> lastSlot = slotList.Last.Value;
		Vector2 slotSize = GetSlotSize();
		int fullSlot = GetFullSlotCount(listData.Count);
		
		int constraintCount = GetContraintSlotCount();
		int nextSlotDataIndex = lastSlot.DataIndex + 1;
		Vector2 nextSlotTop = lastSlot.Top;
		Vector2 nextSlotBottom = lastSlot.Bottom;

		while(nextSlotDataIndex < fullSlot && 
		      nextSlotTop.y >= _visibleRect.y - _visibleRect.height)
		{
			if (nextSlotDataIndex % constraintCount == 0)
			{
				//nextSlotTop = lastSlot.Bottom + new Vector2(0.0f, -slotSize.y);
				nextSlotTop = new Vector2(0.0f, nextSlotBottom.y);
				nextSlotBottom = nextSlotTop + new Vector2(0.0f, -slotSize.y);
			}
			else
			{
				nextSlotTop += new Vector2(slotSize.x + spacingHeight, 0.0f);
				nextSlotBottom = nextSlotTop - new Vector2(0.0f, slotSize.y);
			}
			
			ScrollViewSlot<T> slot = CreateSlotForIndex(nextSlotDataIndex);
			slot.Top = nextSlotTop;
			slot.Bottom = nextSlotBottom;

			lastSlot = slot;
			
			nextSlotDataIndex = lastSlot.DataIndex + 1;
		}
	}
#endregion

#region 셀을 재이용하는 처리 구현
    private Vector2 _prevScrollPos;	// 바로 전의 스크롤 위치를 저장

	// 스크롤 뷰가 스크롤됐을 때 호출된다
	public void OnScrollPosChanged(Vector2 scrollPos)
	{
		// visibleRect를 갱신한다
		UpdateVisibleRect();
        // 스크롤한 방향에 따라 셀을 다시 이용해 표시를 갱신한다
        ReuseSlots((scrollPos.y < _prevScrollPos.y)? 1: -1);

        _prevScrollPos = scrollPos;
	}

    // 셀을 다시 이용해서 표시를 갱신하는 메서드
	private void ReuseSlots(int scrollDirection)
	{
		if(slotList.Count < 1)
		{
			return;
		}

		Vector2 slotSize = GetSlotSize();

		if(scrollDirection > 0)
		{
            // 위로 스크롤하고 있을 때는 visibleRect에 지정된 범위보다 위에 있는 셀을
            // 아래를 향해 순서대로 이동시켜 내용을 갱신한다
            ScrollViewSlot<T> firstSlot = slotList.First.Value;
            int constraintCount = GetContraintSlotCount();

			while(firstSlot.Bottom.y + slotSize.y > _visibleRect.y)
			{
				float topX = slotSize.x / 2;
				
				// constraintCount 만큼 슬롯 생성
				for (int i = 0; i < constraintCount; ++i)
				{
					ScrollViewSlot<T> lastSlot = slotList.Last.Value;
					UpdateSlotForIndex(firstSlot, lastSlot.DataIndex + 1);

					firstSlot.Top = new Vector2(topX, lastSlot.Top.y);
					firstSlot.Bottom = new Vector2(topX, lastSlot.Bottom.y);
					slotList.AddLast(firstSlot);
					slotList.RemoveFirst();
					firstSlot = slotList.First.Value;

					topX += slotSize.x + spacingHeight;
					
					Debug.Log("lastSlot DataIndex: " + lastSlot.DataIndex);
				}
			}

            // visibleRect에 지정된 범위 안에 빈 곳이 있으면 셀을 작성한다
            FillVisibleRectWithSlots();
		}
		else
		{
            // 아래로 스크롤하고 있을 때는 visibleRect에 지정된 범위보다 아래에 있는 셀을
            // 위를 향해 순서대로 이동시켜 내용을 갱신한다
            ScrollViewSlot<T> lastSlot = slotList.Last.Value;
            int constraintCount = GetContraintSlotCount();
            
			while(lastSlot.Top.y + slotSize.y < _visibleRect.y - _visibleRect.height)
			{
				float topX = slotSize.x / 2;
				// constraintCount 만큼 슬롯 생성
				for (int i = 0; i < constraintCount; ++i)
				{
					ScrollViewSlot<T> firstSlot = slotList.First.Value;
					UpdateSlotForIndex(lastSlot, firstSlot.DataIndex - 1);
					//lastSlot.Bottom = new Vector2(topX, firstSlot.Top.y + spacingHeight);
					//lastSlot.Top = new Vector2(topX, firstSlot.Top.y);
					lastSlot.Bottom = new Vector2(topX, firstSlot.Top.y);

					slotList.AddFirst(lastSlot);
					slotList.RemoveLast();
					lastSlot = slotList.Last.Value;

					topX += slotSize.x + spacingHeight;
					
					Debug.Log("firstSlot DataIndex: " + firstSlot.DataIndex);
				}
			}
		}
	}
#endregion
}
