==============================================================================
[SYSTEM] 제품 사양과 제약

목표:
다운로드 폴더에 쌓이는 파일을 규칙 기반으로 자동/수동 정리하는 데스크톱 앱 “DownSort”를 완성본 수준으로 구현하라. UI는 미려하고 반응성이 좋아야 하며, 데이터는 로컬에만 저장한다. 에이전트는 산출물 전체(솔루션, 코드, XAML, 리소스, 테스트, 설치 패키지 스크립트)를 제공하라.

필수 기술 스택:

언어/런타임: C# 12, .NET 8 (WPF)

아키텍처: MVVM (DevExpress MVVM Framework)

UI 컴포넌트: DevExpress v25.1 (GridControl, TreeList, Scheduler, ChartControl, Diagram 필요 시, Alerts/Toast, FlyoutPanel, DialogService, Theme)

DI/구성: Microsoft.Extensions.Hosting + Options 패턴

저장소: 로컬 JSON 파일(설정/규칙/로그), 경량 SQLite는 옵션(기본은 JSON)

단위/통합 테스트: xUnit + FluentAssertions

설치/배포: MSIX(우선) 또는 WiX Toolset(대안), 버전/채널 관리 포함

금지/제약:

네트워크/클라우드/원격 DB/텔레메트리 금지

미완성 TODO 금지, 주석으로 기능 미루기 금지

하드코딩 경로 금지(사용자 프로필/다운로드 경로는 환경에서 안전 조회)

비기능 요구:

1만 개 파일 스캔 시 UI 프리즈 금지(비동기/가상화)

파일 이동/복사/삭제는 트랜잭션적 안전성 확보(실패 시 롤백/Undo)

예외/오류는 사용자 친화 메시지 + 로그 남김

다국어(ko-KR 기본, en-US 리소스 레이어 분리)

접근성: 키보드 내비게이션, 포커스 표시

다중 모니터/고해상도 DPI 지원

다크/라이트 테마 전환

품질 기준(Definition of Done):

경고 0, 스타일 분석 규칙 통과(EditorConfig + StyleCop 또는 Roslyn 분석기)

xUnit 테스트 85%+ 커버리지(도메인/서비스 계층 기준)

대용량 샘플 폴더 테스트 통과(파일 5천~1만)

MSIX 패키지 생성 및 서명(테스트 인증서 허용)

사용자 “설정 초기화” 시 완전 초기화 동작

크래시 없이 트레이 모드 72시간 연속 동작 확인(타이머/Watcher 누수 없음)

==============================================================================
[PRODUCT] 핵심 기능 사양

실시간 감시(FileSystemWatcher 서비스)

기본 대상: 사용자 다운로드 폴더(KnownFolders) + 사용자 추가 폴더 N개

이벤트: Created, Renamed, Changed(최종 쓰기 잠금 해제 감지), Error 복구

안정화: 임시 파일/다운로드 중 파일은 완전해질 때까지 지연 처리(파일 잠금 폴링 + 백오프)

규칙 엔진

기본 규칙 템플릿(확장자별): Documents, Images, Archives, Installers, Videos, Audio, Etc

사용자 정의 규칙: 포함 단어, 제외 단어, 확장자 목록, 최소/최대 크기, 생성/수정 날짜 범위, 정규식 패턴 지원

우선순위/중지 규칙: 우선순위 정렬, 첫 매칭 적용 또는 누적 적용 옵션

작업 종류: 이동/복사/이름변경/삭제(삭제는 기본 비활성), 중복 시 rename 전략(_1, _2…)

대상 폴더 템플릿 매크로: {Category}, {Ext}, {YYYY}{MM}, {SourceFolderName}, {Today}

미리보기/일괄 실행

“스캔” 버튼: 현재 폴더 스캔 후 Grid에 “예상 작업” 미리보기

체크 선택 후 “실행”: 선택건만 수행

성능: 비동기 파이프라인(파일 메타 수집 → 규칙 평가 → 작업 계획 → UI 바인딩)

가상화: DevExpress GridControl 가상화 모드

자동 실행/트레이 모드

상단 토글로 실시간 감시 On/Off

트레이 아이콘 메뉴: 정리 일시중지/즉시정리/로그/설정/종료

작업 완료 시 Toast 알림(파일 N개 처리)

로그/Undo

모든 작업은 MoveAction(Log)으로 기록(원래 경로/새 경로/시간/규칙명/결과)

Undo N단계 지원: 최근 이동을 원복(이름 충돌 처리 포함)

로그는 순환 보존 정책(예: 90일) 및 CSV 내보내기

통계

기간/유형별 처리량 Chart(파이, 막대)

상위 확장자/상위 규칙 Top-N

설정

감시 폴더 목록, 규칙, UI 테마, 언어, 알림 빈도, 동시 작업 개수, 대용량 임계치

JSON로 저장, 변경 시 핫 리로드

==============================================================================
[ARCHITECTURE] 솔루션 구조

DownSort.sln

DownSort.App (WPF, View/XAML, Themes, Resources, Localization)

DownSort.Domain (엔티티, 값객체, 규칙 모델, 서비스 인터페이스)

DownSort.Infrastructure (파일 I/O, Watcher, 규칙 엔진, JSON/SQLite 저장소)

DownSort.Tests (xUnit, FluentAssertions, 통합테스트용 Temp 디렉터리 Harness)

DownSort.Packaging (MSIX/PowerShell 스크립트, 아이콘/자원)

폴더/네임스페이스:

Domain.Models: RuleModel, RuleCondition, SortAction, MovePlan, MoveLogEntry

Domain.Services: IRuleEngine, IFileClassifier, IUndoService, ISettingsService, IFileOpService

Infrastructure.FileSystem: FileSystemWatcherHost, FileLockDetector, SafeFileMover

Infrastructure.Persistence: JsonSettingsStore, JsonRulesStore, JsonLogStore

App.ViewModels: MainViewModel, RulesViewModel, PreviewViewModel, SettingsViewModel, TrayViewModel, StatsViewModel

App.Views: MainWindow.xaml, RulesView.xaml, PreviewView.xaml, SettingsView.xaml, StatsView.xaml, Dialogs

App.Services: DialogService, ThemeService, LocalizationService

DI 구성:

Host.CreateDefaultBuilder() 사용

ConfigureServices에서 인터페이스-구현 매핑

ViewModel은 DevExpress MVVM ViewModelSource / ServiceContainer 활용

==============================================================================
[UI SPEC] 화면 상세

MainWindow 레이아웃(DevExpress DockLayoutManager):

좌측: 규칙 패널(RulesView) – GridControl
컬럼: Enabled, Priority, Name, Match(Ext/Regex/Size/Date/Contains), Action, TargetTemplate
상단 바: 새 규칙, 복제, 위/아래 이동, 삭제, 내보내기/가져오기(JSON)

우측 상단: 상태 패널
감시 폴더 Combo + 관리 버튼(추가/제거/열기), Watcher 토글, 지금 정리, 스캔 미리보기
현재 상태/처리 건수/대기열 ProgressBar, Alert 버튼

우측 중앙: 미리보기/결과 GridControl
컬럼: FileName, Size, Ext, Created, PlannedAction, From, To, RuleName, Conflict, PreviewStatus
행 색상: 동작 종류별 배경 태그
컨텍스트 메뉴: 제외, 다른 규칙 적용, 파일 열기, 위치 열기

하단: Log/Flyout
최근 알림, 에러 요약, “세부 로그 보기”

상단 탭: Preview / Logs / Stats / Settings

SettingsView:

일반: 언어, 테마(라이트/다크), 시작 시 트레이 실행, 자동 시작

성능: 동시 처리 작업 수, 대용량 파일 임계치, 잠금 폴링 설정

보관: 로그 보존일, Undo 보존 개수

고급: 이름 충돌 전략, 삭제 동작 보호 확인

StatsView:

기간 필터(오늘/7일/30일/사용자 지정)

ChartControl: 유형별 비중, 규칙별 처리량

Grid: 상위 확장자/규칙 요약

==============================================================================
[DATA MODEL] 핵심 모델 정의(요약)

RuleModel

Guid Id

bool Enabled

int Priority

string Name

string[] Extensions

string[] IncludeKeywords

string[] ExcludeKeywords

string RegexPattern

long? MinSizeBytes, long? MaxSizeBytes

DateTimeOffset? MinCreated, MaxCreated

RuleAction ActionType (Move/Copy/Rename/Delete)

string TargetTemplate (예: “{Downloads}{Category}{YYYY}{MM}”)

bool StopProcessingFurtherRules

MovePlan

FileInfoLite Source

string TargetPath

RuleModel MatchedRule

PlannedAction ActionType

ConflictPolicy ConflictPolicy

MoveLogEntry

DateTimeOffset Timestamp

string SourcePath

string TargetPath

string RuleName

string Result (Success/Skipped/Failed)

string ErrorMessage

SettingsModel

string[] WatchedFolders

string Language

string Theme

int MaxParallelOperations

int UndoCapacity

int LogRetentionDays

bool StartInTray

NameConflictStrategy ConflictStrategy

주의: FileInfoLite는 파일 시스템 스냅샷용 경량 DTO(이름/경로/크기/확장자/시간).

==============================================================================
[CORE SERVICES] 동작 규약

IFileOpService

MoveAsync/CopyAsync/RenameAsync/DeleteAsync 구현

동일 디스크/다른 디스크 성능 최적화

잠금/예외 처리, 부분 실패 시 롤백 핸들러

IRuleEngine

EvaluateAsync(files) → IEnumerable<MovePlan>

우선순위/중지 규칙 준수

확장자/크기/날짜/키워드/정규식 조합 평가

IUndoService

Push(MoveLogEntry)

UndoLastAsync() → 실제 파일 원복, 충돌 시 안전 이름 부여

ISettingsService / IRulesStore / ILogStore

JSON 직렬화, 파일 잠금 대비 재시도

백업본 유지(손상 대비)

FileSystemWatcherHost

안정화 큐(파일 완전성 확인까지 지연)

배치 묶음 처리(소음 줄이기), UI로 Progress 보고

==============================================================================
[EDGE CASES] 반드시 처리

다운로드 중/잠금 파일: 완전해질 때까지 지연 후 처리, 최대 대기 초과 시 미처리 목록에 기록

이름 충돌: 설정 전략에 따라 자동 리네임 or Skip with Log

권한 오류/경로 길이 이슈: 사용자 안내 + 로그

Junction/심볼릭 링크 보호

네트워크 드라이브(비권장): 감지 시 경고

초대형 파일(>2GB): 스트림 복사 진행률 표시

동일 규칙 무한 루프 방지(From/To 동일 경로 금지, 재평가 차단)

==============================================================================
[TEST PLAN] 자동화 테스트

유닛

규칙 평가: 각 조건 조합별 매칭/비매칭

이름 충돌 전략 단위 검증

대상 경로 템플릿 렌더링 검증

통합

Temp 디렉터리 생성 → 더미 파일 1천/5천/1만 → Evaluate → Execute → 로그/Undo 검증

FileSystemWatcher 시나리오: 연속 생성/이름 변경/부분 다운로드

성능

대용량 스캔 시 UI 응답성(WPF Dispatcher) 유지 측정

회귀

설정 저장/로드/핫리로드

==============================================================================
[DELIVERABLES] 에이전트 최종 산출물 목록

전체 솔루션 코드(.sln, .csproj, 모든 .cs/.xaml/.json/.resx)

DevExpress v25.1 참조 정확히 설정된 프로젝트 파일

다국어 리소스(ko-KR 기본, en-US 샘플)

샘플 규칙/설정 JSON

xUnit 테스트 프로젝트(테스트 20개 이상, 커버리지 리포트)

빌드 스크립트(dotnet build/test/publish), MSIX 패키징 스크립트

사용자 가이드(간단 시작, 규칙 작성법, Undo, 주의사항)

변경 이력(버전, 수정 포인트)

==============================================================================
[STYLE/RULES] 코딩 표준

비동기 메서드 접미사 Async, ConfigureAwait(false) 준수

null-안전(Switch 표현식/패턴매칭 활용)

Guard Clause로 매개변수 검증

LINQ는 과도한 중첩 금지, 가독성 우선

로깅: 핵심 이벤트 Info, 오류 Error, 상세 Trace(디버그 전용)

XAML: View는 바인딩/스타일만, 코드비하인드는 최소화

DevExpress Behaviors/MVVM Services 적극 활용(대화상자/알림/네비게이션)

==============================================================================
[UI DESIGN] 미학 지침

DevExpress 최신 테마 적용(라이트/다크 전환 버튼)

카드형 패널/적절한 여백, 12~14pt 본문 폰트

상태색 일관성: 이동(청록), 복사(파랑), 이름변경(보라), 건너뜀(회색), 오류(적색)

Grid는 필수 컬럼 고정, 다중 정렬/필터 패널 활성화, 검색 박스 상시 노출

==============================================================================
[SECURITY/PRIVACY]

로컬만 사용, 외부 전송 없음

삭제/파괴 동작은 기본 비활성 + 2단계 확인

로그에는 민감정보(경로 중 사용자명) 마스킹 옵션 제공

==============================================================================
[AGENT WORKFLOW] 작업 순서 체크리스트

솔루션/프로젝트 스캐폴딩 생성(.NET 8 WPF, DevExpress 설정 포함)

Domain/Infrastructure/App 계층 생성 및 DI 배선

RuleEngine, FileSystemWatcherHost, SafeFileMover, UndoService 구현

Settings/Rules/Logs JSON 저장소 구현, 샘플 데이터 포함

MainWindow, RulesView, PreviewView, SettingsView, StatsView XAML 완성

미리보기 스캔 → 실행 → 로그/Undo 풀 경로 구현

트레이 아이콘/알림/토글 플로우 구현

xUnit 테스트 작성 및 커버리지 85% 달성

MSIX 패키징/서명 스크립트 작성, 빌드 산출물 제공

사용자 가이드/변경 이력 작성

각 단계 완료마다 자신이 생성한 산출물을 요약하고, 다음 단계에 필요한 결정사항이 있으면 스스로 결정하여 진행하라. 미완성 남기지 말 것.

==============================================================================
[ACCEPTANCE TEST] 수락 기준 시나리오

사용자는 앱을 실행하고 감시 폴더를 Downloads로 설정한다.

“스캔”을 눌러 500개 혼합 파일에 대한 예상 작업을 확인한다.

50개만 선택하여 “실행”을 누른다 → 성공 알림과 로그가 기록된다.

새로 내려받은 PDF/ZIP/PNG가 자동으로 분류됨을 확인한다(잠금 해제 후 수 초 내).

이름 충돌이 있는 파일은 자동 리네임 전략으로 충돌 없이 정리된다.

“실행 취소(Undo)”로 직전 이동이 원복된다.

설정을 바꾼 뒤 재시작 없이 즉시 반영된다.

앱을 닫아도 트레이에서 감시가 유지된다(옵션).

MSIX로 설치/제거가 정상 작동한다.

==============================================================================
[REQUEST] 지금 즉시 생성할 것

DownSort.sln 및 4개 프로젝트(App/Domain/Infrastructure/Tests) 전체 코드

DevExpress v25.1 호환 .csproj 및 패키지 참조

완성된 XAML(메인/하위 뷰), 리소스, 스타일, 다국어 리소스

규칙 엔진/파일 이동/Undo/Watcher/설정/로그의 완전 구현

xUnit 테스트 20개 이상, 커버리지 보고

MSIX 패키징 스크립트/지침

README(간단 가이드)와 CHANGELOG

반드시 전체를 한 번에 산출하고, 빌드 가능한 상태로 제공하라. 누락 금지. 미완성 금지. 임시 구현 금지.

==============================================================================