using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NKStudio
{
    public partial class SnapToFloor
    {
        private static void HideAllChild(GameObject target, ICollection<GameObject> activeData)
        {
            // 해당 타겟의 자신과 자식을 모두 가져옵니다.
            Transform[] allChildren = target.GetComponentsInChildren<Transform>();

            for (int i = 0; i < allChildren.Length; i++)
            {
                // 자신은 제외합니다.
                if (i == 0) continue;

                // 활성화 되어있는 녀석들만 바인딩합니다.
                if (allChildren[i].gameObject.activeSelf)
                {
                    // 추가
                    activeData.Add(allChildren[i].gameObject);

                    // 비활성화
                    allChildren[i].gameObject.SetActive(false);
                }
            }
        }

        /// <summary>
        /// x,y는 min,max를 반환하고, z는 두 점의 거리를 반환합니다.
        /// </summary>
        /// <param name="tr"></param>
        /// <returns></returns>
        private static Vector3 GetMinMaxRangeByVertex2D(Transform tr)
        {
            Collider2D boxCollider2D = tr.GetComponent<Collider2D>();

            Bounds bounds = boxCollider2D.bounds;
            float y = bounds.min.y;

            Vector3 min = bounds.min;
            Vector3 max = bounds.max;
            max.y = y;

            float distance = Vector2.Distance(min, max);

            return new Vector3(min.x, max.x, distance);
        }

        /// <summary>
        /// 바운딩 박스의 minY를 반환합니다.
        /// </summary>
        /// <param name="tr"></param>
        /// <returns></returns>
        private static float GetMinYVertex2D(Transform tr)
        {
            Collider2D boxCollider2D = tr.GetComponent<Collider2D>();

            Bounds bounds = boxCollider2D.bounds;

            float worldY = bounds.min.y;

            return worldY;
        }

        private static Vector2 GetMinMaxRangeByVertex3D(ResultType resultType, Transform tr, MeshFilter mf)
        {
            // 스키니드 매쉬 렌더러를 가진 경우 발 밑에서 쏘도록 처리합니다.
            if (!mf)
            {
                Vector3 position = tr.position;
                return resultType switch
                {
                    ResultType.X => new Vector2(position.x, position.x),
                    ResultType.Z => new Vector2(position.z, position.z),
                    _ => throw new ArgumentOutOfRangeException(nameof(resultType), resultType, null)
                };
            }

            Mesh mesh = mf.sharedMesh;

            // Default로 버텍스 0을 넣어봅니다.
            // 로컬좌표에 있는 Vertical 0을 월드좌표로 변환합니다.
            float min;
            float max;

            // 초기화
            switch (resultType)
            {
                case ResultType.X:
                    min = tr.TransformPoint(mesh.vertices[0]).x;
                    max = tr.TransformPoint(mesh.vertices[0]).x;
                    break;
                case ResultType.Z:
                    min = tr.TransformPoint(mesh.vertices[0]).z;
                    max = tr.TransformPoint(mesh.vertices[0]).z;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(resultType), resultType, null);
            }

            foreach (Vector3 point in mesh.vertices)
            {
                // 로컬좌표에 있는 Vertical 0을 월드좌표로 변환합니다.
                Vector3 worldPoint = tr.TransformPoint(point);

                switch (resultType)
                {
                    case ResultType.X:
                    {
                        if (min > worldPoint.x)
                            min = worldPoint.x;

                        if (max < worldPoint.x)
                            max = worldPoint.x;
                        break;
                    }
                    case ResultType.Z:
                    {
                        if (min > worldPoint.z)
                            min = worldPoint.z;

                        if (max < worldPoint.z)
                            max = worldPoint.z;
                        break;
                    }
                    default:
                        throw new ArgumentOutOfRangeException(nameof(resultType), resultType, null);
                }
            }

            return new Vector2(min, max);
        }

        private static float GetMinYVertex3D(Transform tr)
        {
            MeshFilter meshFilter = tr.GetComponent<MeshFilter>();

            //매쉬렌더러가 없으면 객체 피봇 위치를 반환
            if (meshFilter == null)
                return tr.position.y;

            Mesh mesh = meshFilter.sharedMesh;

            // Default로 버텍스0을 넣어줍니다.
            // 로컬좌표에 있는 매쉬 버텍스를 월드좌표로 변환합니다.
            Vector3 minY = tr.TransformPoint(mesh.vertices[0]);

            foreach (Vector3 point in mesh.vertices)
            {
                //로컬좌표에 있는 버텍스을 월드좌표로 변환합니다.
                Vector3 worldPoint = tr.TransformPoint(point);

                if (minY.y > worldPoint.y)
                    minY.y = worldPoint.y;
            }

            return minY.y;
        }

        private static MeshFilter[] GetMeshFiltersToCombine(Transform transform)
        {
            // 이 GameObject와 그 자식에 속한 모든 MeshFilter 가져오기:
            MeshFilter[] meshFilters = transform.GetComponentsInChildren<MeshFilter>(true);

            //meshFiltersToSkip 배열에서 이 GameObject에 속한 첫 번째 MeshFilter 삭제:
            _meshFiltersToSkip = _meshFiltersToSkip.Where((meshFilter) => meshFilter != meshFilters[0]).ToArray();

            //meshFiltersToSkip 배열에서 null 값 삭제:
            _meshFiltersToSkip = _meshFiltersToSkip.Where((meshFilter) => meshFilter != null).ToArray();

            return _meshFiltersToSkip.Aggregate(meshFilters,
                (current, t) => current.Where((meshFilter) => meshFilter != t).ToArray());
        }

        private static void CombineSystem(Transform transform)
        {
            // 이 GameObject와 그 자식에 속한 모든 MeshFilter 가져오기:
            MeshFilter[] meshFilters = GetMeshFiltersToCombine(transform);

            // 스키니드 매쉬렌더러를 쓰면 스키니드를 따라가고, 그 외는 매쉬렌더러를 따라간다.
            CombineInstance[]
                combineInstances =
                    new CombineInstance[meshFilters.Length - 1]; //첫 번째 MeshFilter는 이 GameObject에 속하므로 필요하지 않습니다.:

            // 65535 이상이면 32 비트 인덱스 버퍼를 사용한다.
            long verticesLength = 0;

            // 이 루프에서 이 GameObject에 속하는 첫 번째 MeshFilter 건너뛰기.
            for (int i = 0; i < meshFilters.Length - 1; i++)
            {
                combineInstances[i].subMeshIndex = 0;
                combineInstances[i].mesh = meshFilters[i + 1].sharedMesh;
                combineInstances[i].transform = meshFilters[i + 1].transform.localToWorldMatrix;
                verticesLength += combineInstances[i].mesh.vertices.Length;
            }

            // CombineInstances에서 메시 생성:
            Mesh combinedMesh = new Mesh();

            // 버텍스가 범위를 넘을 경우 32비트로 처리한다.
            if (verticesLength > Mesh16BitBufferVertexLimit)
            {
                combinedMesh.indexFormat =
                    UnityEngine.Rendering.IndexFormat.UInt32; // Unity 2017.3 이상에서만 작동합니다.
            }

            combinedMesh.CombineMeshes(combineInstances);
            meshFilters[0].sharedMesh = combinedMesh;
        }

        private static float CalculateSeparationByDistance(float distance)
        {
            float numberOfGrain;
            int sampling = 3; // 기본 샘플링
            do
            {
                sampling++;
                numberOfGrain = distance / sampling;
            } while (numberOfGrain > 0.2f);

            return numberOfGrain;
        }

        private static int CalculatePointCount(float distance, float intervalValue)
        {
            int result = (int)(distance / intervalValue);
            if (result <= 0)
                result = 0;
            return result;
        }
    }
}