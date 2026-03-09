# PagingTemplate

Unity 키오스크용 페이징 UI 템플릿. **MVP + FSM + DataRepository** 패턴 사용.
자세한 내용은 [README.md](README.md) 참조.

---

## 코딩 컨벤션

- **필드**: `_camelCase` (private), `[SerializeField]` 적용
- **클래스/메서드**: `PascalCase`
- **싱글톤**: 반드시 `MonoSingleton<T>` 상속
- **새 화면 추가 패턴**:
  1. `XxxView : BaseView` 생성
  2. `XxxState : BaseState<XxxState, XxxView>` 생성
  3. `NavigationManager.RegisterState()`에 등록
  4. `DataConfig.json`에 CSV 파일 항목 추가
- **이벤트 구독**: `Init()`에서 구독, `Dispose()`에서 해제
- **View 데이터 바인딩**: `BindView()` override에서 처리

---

## 개선 예정 사항 (TODO)

- [x] `DataRepository`: CSV 파일명 하드코딩 → `DataConfig.json` 기반으로 교체 완료
  - View당 여러 CSV 파일 지원, 키 충돌 시 후순위 파일이 덮어씀
  - JSON 파싱: `JsonUtility` (Unity 내장) 사용
- [ ] `CSVParser`: 동기 파일 읽기 → 비동기(`UnityWebRequest`) 전환 검토

