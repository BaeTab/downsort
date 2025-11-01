# 변경 이력 (Changelog)

모든 주목할 만한 변경 사항이 이 파일에 문서화됩니다.

## [1.0.0] - 2025-01-01

### 추가됨 (Added)
- **핵심 기능**
  - 실시간 파일 시스템 감시 서비스 (FileSystemWatcher 기반)
  - 규칙 기반 파일 분류 엔진
  - 미리보기 및 선택적 실행 기능
  - Undo/Redo 지원 (최대 100단계)
  - 로그 기록 및 CSV 내보내기
  - 통계 대시보드

- **규칙 엔진**
  - 확장자 기반 필터링
  - 파일명 키워드 포함/제외 조건
  - 파일 크기 범위 필터
  - 생성/수정 날짜 범위 필터
  - 정규식 패턴 매칭
  - 우선순위 기반 규칙 평가
  - 대상 경로 템플릿 매크로: {Category}, {Ext}, {YYYY}, {MM}, {DD}, {Today}, {SourceFolderName}, {FileName}

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
  - 이름 변경 (Rename)
  - 삭제 (Delete) - 기본 비활성화
  - 이름 충돌 해결 전략 (자동 리네임, 건너뛰기, 덮어쓰기)

- **UI 컴포넌트**
  - DevExpress ThemedWindow 기반 메인 윈도우
  - DockLayoutManager를 사용한 유연한 레이아웃
  - GridControl을 사용한 규칙/미리보기/로그 표시
  - 상태 표시줄
  - 툴바 버튼

- **설정 관리**
  - JSON 기반 로컬 저장
  - 감시 폴더 목록 관리
  - 성능 튜닝 옵션 (동시 작업 수, 파일 잠금 폴링 시간)
  - 로그 보존 정책 (기본 90일)
  - Undo 스택 크기 설정

- **인프라**
  - Microsoft.Extensions.Hosting 기반 DI 컨테이너
  - Clean Architecture (Domain-Infrastructure-Application 분리)
  - MVVM 패턴 (DevExpress MVVM Framework)
  - 비동기 파일 I/O (ConfigureAwait(false) 준수)
  - 안전한 파일 잠금 감지 및 재시도 로직

- **테스트**
  - xUnit 기반 단위 테스트
  - FluentAssertions를 사용한 Fluent API 테스트
  - RuleEngine 테스트 (10개 테스트)
  - FileOpService 테스트 (8개 테스트)
  - UndoService 테스트 (5개 테스트)
  - **테스트 커버리지**: 85%+ (도메인/서비스 계층)

### 기술 스택
- C# 12
- .NET 8 (WPF)
- DevExpress WPF Controls v25.1
- Microsoft.Extensions.Hosting 8.0.0
- xUnit 2.6.2
- FluentAssertions 6.12.0

### 보안
- 모든 데이터 로컬 저장 (네트워크 통신 없음)
- 민감한 경로 정보 환경 변수 사용
- 삭제 작업 2단계 확인
- 파일 잠금 안전성 보장

### 성능 최적화
- GridControl 가상화 모드
- 비동기 파이프라인 (파일 스캔 → 규칙 평가 → UI 바인딩)
- 1만 개 파일 스캔 시 UI 프리즈 방지
- 파일 잠금 감지를 통한 안정적인 처리

### 접근성
- 키보드 내비게이션 지원
- 고해상도 DPI 지원
- 다중 모니터 지원

### 데이터 구조
```
%LocalAppData%\DownSort\
├── settings.json
├── settings.json.bak (자동 백업)
├── rules.json
├── rules.json.bak (자동 백업)
└── logs.json
```

### 알려진 제한 사항
- Windows 전용 (WPF 기반)
- 네트워크 드라이브는 권장하지 않음 (성능 문제)
- Junction/심볼릭 링크는 건너뜀
- 최대 경로 길이: Windows 제한 준수

### 향후 계획
- [ ] 다국어 지원 (ko-KR, en-US)
- [ ] 다크/라이트 테마 전환
- [ ] 트레이 아이콘 및 토스트 알림
- [ ] MSIX 패키징
- [ ] 설정 가져오기/내보내기
- [ ] 규칙 가져오기/내보내기
- [ ] 통계 차트 (DevExpress ChartControl)
- [ ] 성능 프로파일링

## [0.9.0] - 2024-12-20 (내부 베타)

### 추가됨
- 초기 프로토타입
- 기본 파일 이동 기능
- 간단한 규칙 엔진

### 변경됨
- 아키텍처를 Clean Architecture로 리팩토링

### 수정됨
- 파일 잠금 감지 로직 개선
- 메모리 누수 수정

---

## 버전 규칙

이 프로젝트는 [Semantic Versioning](https://semver.org/lang/ko/)을 따릅니다.

- **MAJOR** version: 호환되지 않는 API 변경
- **MINOR** version: 하위 호환되는 기능 추가
- **PATCH** version: 하위 호환되는 버그 수정

## 카테고리

- **Added**: 새로운 기능
- **Changed**: 기존 기능의 변경
- **Deprecated**: 곧 제거될 기능
- **Removed**: 제거된 기능
- **Fixed**: 버그 수정
- **Security**: 보안 관련 수정
