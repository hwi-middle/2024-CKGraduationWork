using System.Collections.Generic;
using UnityEngine;

namespace NKStudio
{
    public partial class SnapToFloor
    {
        /// <summary>
        /// 선택한 객체를 땅에 스냅을 시킵니다.
        /// </summary>
        /// <param name="transform">스냅시킬 대상</param>
        private static void SnapToFloor2D(Transform transform)
        {
            List<GameObject> activeData = new();

            Collider2D hasCollider = transform.GetComponent<Collider2D>();

            // 경고 메세지
            if (hasCollider == null)
            {
                if (Application.systemLanguage == SystemLanguage.Korean)
                    Debug.LogError("collider 2D를 찾을 수 없습니다.");
                else
                    Debug.LogError("Could not find collider 2D");

                return;
            }

            HideAllChild(hasCollider.gameObject, activeData);

            #region 매쉬의 버텍스에 대한 월드 계산 위치

            Vector3 minMaxDistance = GetMinMaxRangeByVertex2D(transform);

            float footYPosition = GetMinYVertex2D(transform);

            #endregion

            float distance = minMaxDistance.z;

            float startPosition = minMaxDistance.x;

            // 간격에 따른 알맞는 간격을 계산한다.
            float intervalValue = CalculateSeparationByDistance(distance);

            // 간격에 알맞는 알갱이를 가져옴
            int numberOfGrains = CalculatePointCount(distance, intervalValue);
            int nowNumberOfGrains = numberOfGrains + 1;

            Vector3 position = transform.position;
            float? moveY = null;

            // 원하는 알갱이 수 만큼 반복한다
            // 원래는 내가 원하는 간격을 제시하면 그것에 맞는 알갱이를 뿌린다.
            for (int i = 0; i < nowNumberOfGrains; i++)
            {
                // 그려낼 위치에서 사이간격에 맞춰 그려냄
                float xx = startPosition + intervalValue * i;

                Vector3 drawPosition = new Vector3(xx, footYPosition, position.z);

                //각각의 오브젝트의 위치에서 아래 방향으로 Ray를 쏜다.
                RaycastHit2D[] hits = Physics2D.RaycastAll(drawPosition, Vector2.down, Height);

                // 각각 hit정보 확인
                foreach (RaycastHit2D hit in hits)
                {
                    // 자기 자신의 콜라이더를 맞춘 경우 pass : 예외 처리
                    if (hit.collider.gameObject == transform.gameObject)
                        continue;

                    if (moveY == null)
                        moveY = hit.distance;
                    else
                    {
                        if (moveY > hit.distance)
                            moveY = hit.distance;
                    }
                }
            }

            position.y -= moveY ?? 0f;
            // hit된 위치로 이동시킨다.
            transform.position = position;

            // 다시 활성화 시킵니다.
            foreach (GameObject o in activeData)
                o.SetActive(true);
        }
    }
}