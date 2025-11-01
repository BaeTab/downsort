# Changelog (변경 이력)

모든 주목할 만한 변경 사항은 이 파일에 문서화됩니다.

이 프로젝트는 [Semantic Versioning](https://semver.org/lang/ko/)을 따릅니다.

---

## [1.0.2] - 2025-01-10

### 추가됨 (Added)
- **자동 업데이트 기능**
  - GitHub Releases API 연동
  - 시작 시 자동 업데이트 확인 (24시간 간격)
  - 수동 업데이트 확인 버튼
  - Material Design 업데이트 다이얼로그
  - 진행률 표시와 함께 다운로드
  - 자동 설치 (관리자 권한 요청)

- **설정 추가**
  - `CheckForUpdatesOnStartup`: 시작 시 자동 업데이트 확인
  - `LastUpdateCheck`: 마지막 업데이트 확인 시간

- **새로운 서비스**
  - `IUpdateService`: 업데이트 서비스 인터페이스
  - `UpdateService`: GitHub API 구현
  - `ReleaseInfo`: 릴리스 정보 모델
  - `ReleaseAsset`: 다운로드 파일 정보

### 개선됨 (Improved)
- MainWindow UI에 Update 버튼 추가
- 버전 정보를 Directory.Build.props로 중앙 관리
- 문서 구조 개선

### 기술 상세
- GitHub API를 통한 최신 릴리스 확인
- 버전 비교 (v1.0.1 vs v1.0.2)
- 설치 프로그램 자동 다운로드
- Temp 폴더에 다운로드 저장
- 설치 후 애플리케이션 자동 재시작

---

## [1.0.1] - 2025-01-08

### 추가됨 (Added)
- **커스텀 타이틀바**
  - Material Design 스타일 타이틀바
  - 창 드래그로 이동
  - 더블클릭으로 최대화/복원
  - 호버 효과가 있는 창 컨트롤 버튼
  - 앱 아이콘과 부제목 표시

### 개선됨 (Improved)
- DevExpress NuGet 설정 간소화
- 빌드 프로세스 문서화
- 수동 빌드로 전환 (CI/CD 제거)

### 수정됨 (Fixed)
- 창 관리 관련 버그 수정
- API 키 노출 보안 문제 해결

---

## [1.0.0] - 2025-01-01

### 추가됨 (Added)
- **핵심 기능**
  - 실시간 파일 시스템 감시 (FileSystemWatcher 기반)
  - 규칙 기반 자동 분류 시스템
  - 미리보기 및 수동 실행 기능
  - Undo/Redo 기능 (최대 100단계)
  - 로그 관리 및 CSV 내보내기
  - 통계 대시보드

- **규칙 엔진**
  - 확장자 기반 필터링
  - 파일명 키워드 포함/제외 조건
  - 파일 크기 범위 조건
  - 생성/수정 날짜 범위 조건
  - 정규식 패턴 매칭
  - 우선순위 기반 규칙 적용
  - 대상 폴더 템플릿 매크로: {Category}, {Ext}, {YYYY}, {MM}, {DD}

- **기본 규칙 템플릿**
  - Documents (pdf, doc, docx, xls, xlsx, ppt, pptx, txt, rtf, odt)
  - Images (jpg, jpeg, png, gif, bmp, svg, webp, ico)
  - Archives (zip, rar, 7z, tar, gz, bz2)
  - Installers (exe, msi, dmg, pkg)
  - Videos (mp4, avi, mkv, mov, wmv, flv, webm)
  - Audio (mp3, wav, flac, aac, ogg, wma, m4a)

- **파일 작업**
  - 이동 (Move)
  - 복사 (Copy)
  - 삭제 (Delete) - 기본 비활성화
  - 이름 충돌 해결 전략 (자동 리네임, 건너뛰기, 덮어쓰기)

- **UI 컴포넌트**
  - DevExpress ThemedWindow 기반 메인 윈도우
  - DockLayoutManager로 구성된 유연한 레이아웃
  - GridControl로 구성된 규칙/미리보기/로그 표시
  - 실시간 상태 표시
  - Material Design 아이콘

- **설정 관리**
  - JSON 기반 설정 저장
  - 감시 폴더 다중 선택
  - 충돌 해결 전략 선택
  - 로그 보존 정책 (기본 90일)
  - Undo 스택 크기 설정

- **아키텍처**
  - Microsoft.Extensions.Hosting 기반 DI 컨테이너
  - Clean Architecture (Domain-Infrastructure-Application 분리)
  - MVVM 패턴 (DevExpress MVVM Framework)
  - 비동기 파일 I/O
  - 안정적인 파일 잠금 감지

- **테스트**
  - xUnit 기반 단위 테스트
  - FluentAssertions 기반 테스트
  - RuleEngine 테스트 (10개)
  - FileOpService 테스트 (8개)
  - UndoService 테스트 (5개)
  - **테스트 커버리지**: 85%+

### 기술 스택
- C# 12
- .NET 8 (WPF)
- DevExpress WPF Controls v25.1
- Microsoft.Extensions.Hosting 8.0.0
- xUnit 2.6.2
- FluentAssertions 6.12.0

### 보안
- 모든 데이터 로컬 저장 (네트워크 통신 없음)
- 민감한 작업 2단계 확인
- 설정 파일 백업 자동 생성

### 데이터 저장 위치
```
%LocalAppData%\DownSort\
├── settings.json
├── settings.json.bak
├── rules.json
├── rules.json.bak
└── logs.json
```

### 알려진 제한 사항
- Windows 전용 (WPF 기반)
- 네트워크 드라이브 성능 저하 가능
- Junction/심볼릭 링크 건너뜀

---

## [0.9.0] - 2024-12-20 (내부 베타)

### 추가됨
- 초기 프로토타입
- 기본 파일 이동 기능
- 간단한 규칙 엔진

### 변경됨
- 아키텍처 Clean Architecture로 리팩터링

---
