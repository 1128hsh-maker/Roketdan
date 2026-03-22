using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CellType
{
    Blocked,    // 아예 사용 불가
    Buildable,  // 설치 가능
    Locked,     // 잠금 상태
    Path        // 몬스터 이동 경로
}
