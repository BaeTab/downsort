# DownSort

**Smart File Organizer for Windows**

<div align="center">

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![WPF](https://img.shields.io/badge/WPF-Windows-0078D4?logo=windows)](https://docs.microsoft.com/dotnet/desktop/wpf/)
[![DevExpress](https://img.shields.io/badge/DevExpress-25.1-FF7200)](https://www.devexpress.com/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE.txt)
[![Release](https://img.shields.io/github/v/release/BaeTab/downsort)](https://github.com/BaeTab/downsort/releases/latest)

다운로드 폴더에 쌓이는 파일들을 규칙 기반으로 자동/수동 정리하는 데스크톱 애플리케이션

[다운로드](https://github.com/BaeTab/downsort/releases/latest) · [사용 가이드](USER_GUIDE.md) · [문제 해결](Setup/TROUBLESHOOTING.md) · [변경 이력](CHANGELOG.md)

</div>

---

## 주요 기능

### 실시간 파일 감시
- 다운로드 폴더를 실시간으로 모니터링
- 새 파일 생성 시 자동으로 규칙에 따라 정리
- 파일 잠금 감지 및 다운로드 완료 후 처리

### 규칙 기반 자동 분류
- 6개 기본 규칙 제공 (Documents, Images, Archives, Videos, Audio, Installers)
- 사용자 정의 규칙 생성
  - 확장자 필터
  - 파일명 키워드 (포함/제외)
  - 파일 크기 범위
  - 생성/수정 날짜 범위
  - 정규식 패턴
- 우선순위 설정
- 대상 폴더 템플릿 매크로: `{Category}`, `{YYYY}`, `{MM}`, `{Ext}` 등

### 미리보기 및 수동 실행
- "Scan" 버튼으로 현재 폴더 분석
- DevExpress GridControl로 예상 작업 표시
- 선택적으로 특정 파일만 실행

### 실시간 활동 로그 (NEW!)
- 처리된 파일을 Recent Activity 패널에 실시간 표시
- 색상 코딩: 성공(녹색), 실패(빨간색), 건너뛰기(주황색)
- 더블클릭 또는 "Open Folder" 버튼으로 파일 위치로 즉시 이동
- 최근 100개 항목 자동 유지

### 실행 취소 (Undo)
- 최근 이동 작업을 원복
- 설정 가능한 Undo 스택 (기본 100개)
- 이름 충돌 처리 포함

### 로그 및 통계
- 모든 작업 로그 기록
- 순환 보존 정책 (기본 90일)
- 기간별/유형별 처리량 통계

---

## 스크린샷

### 메인 화면 (Modern Material Design)

![DownSort Main Window](https://github.com/user-attachments/assets/a209e3a2-b624-4449-b9b4-d34eca1c4891)

> **스크린샷 추가 방법**:
> 1. 스크린샷 캡처 (Windows + Shift + S)
> 2. GitHub Issues 페이지로 이동
> 3. 새 Issue 작성 창에 이미지 붙여넣기 (Ctrl + V)
> 4. 자동 생성된 URL 복사하여 README에 추가

### 주요 디자인 특징
- Material Design 색상 팔레트
- SVG 벡터 아이콘 (Watch, Scan, Execute, Undo, Clear)
- 그라디언트 & 그림자 효과
- 색상 코딩 시스템 (Success/Failed/Skipped)
- 실시간 상태 표시 (Live Status Indicator)
- 반응형 레이아웃 (DockLayoutManager)

---

## 설치

### 옵션 1: Windows 설치 프로그램 (권장)

**[최신 릴리스 다운로드](https://github.com/BaeTab/downsort/releases/latest)** - v1.0.0

1. `DownSort-Setup-1.0.0.exe` 다운로드 (57.48 MB)
2. 실행 파일 더블클릭
3. 설치 마법사 따라하기

**요구사항**:
- Windows 10 (64-bit) 이상
- .NET 8 Runtime (설치 시 자동 안내)

### 옵션 2: 수동 설치 (ZIP)

1. [DownSort-v1.0.0-win-x64.zip](https://github.com/BaeTab/downsort/releases/latest) 다운로드 (80.59 MB)
2. 원하는 폴더에 압축 해제
3. `Downsort.exe` 실행

---

## 빠른 시작

### 1. 첫 실행
```
애플리케이션 실행 -> 기본 규칙 자동 생성 -> 감시 폴더 = 다운로드 폴더
```

### 2. 자동 정리 활성화
```
"Watch" 버튼 클릭 -> 실시간 감시 시작 -> 새 파일 자동 정리
```

### 3. 수동 정리
```
"Scan" 버튼 -> 미리보기 확인 -> 파일 선택 -> "Execute" 클릭
```

### 4. 활동 확인
```
Recent Activity 패널에서 처리된 파일 확인 -> 더블클릭으로 폴더 열기
```

더 자세한 내용은 [사용자 가이드](USER_GUIDE.md)를 참조하세요.

---

## 개발자용

### 기술 스택

| 기술 | 버전 | 용도 |
|------|------|------|
| .NET | 8.0 | 최신 .NET 플랫폼 |
| WPF | - | Windows UI 프레임워크 |
| DevExpress | 25.1 | 고급 UI 컨트롤 (GridControl, DockLayoutManager) |
| MVVM | - | DevExpress MVVM Framework |
| Microsoft.Extensions | 9.0 | 의존성 주입, 구성 관리 |
| xUnit | 2.6 | 단위 테스트 |
| FluentAssertions | 6.12 | 테스트 어설션 |

### 프로젝트 구조

```
DownSort.sln
├── Downsort/                    # WPF 애플리케이션 (Presentation)
│   ├── ViewModels/              # MVVM ViewModels
│   ├── MainWindow.xaml          # 메인 UI (DevExpress ThemedWindow)
│   └── App.xaml.cs              # DI 및 애플리케이션 진입점
├── DownSort.Domain/             # 도메인 레이어
│   ├── Models/                  # 엔티티 및 값 객체
│   └── Services/                # 서비스 인터페이스
├── DownSort.Infrastructure/     # 인프라 레이어
│   ├── FileSystem/              # 파일 작업 및 감시 서비스
│   ├── Persistence/             # JSON 저장소
│   ├── Rules/                   # 규칙 엔진
│   └── Services/                # 서비스 구현
├── DownSort.Tests/              # 단위 및 통합 테스트 (23개)
└── Setup/                       # 설치 프로그램 스크립트
    ├── DownSort.iss             # Inno Setup 스크립트
    ├── build-installer.ps1      # 빌드 자동화
    └── build.bat                # 간편 빌드
```

### 빌드 및 실행

#### 요구사항
- Visual Studio 2022 이상
- .NET 8 SDK
- DevExpress WPF Controls v25.1 (NuGet)

#### 빌드
```bash
# 복원 및 빌드
dotnet restore
dotnet build

# 테스트 실행
dotnet test

# 애플리케이션 실행
dotnet run --project Downsort/Downsort.csproj
```

#### 설치 프로그램 생성
```powershell
cd Setup
.\build-installer.ps1 -CreateInstaller
```

생성된 파일:
- `Installer/DownSort-Setup-1.0.0.exe` (57.48 MB)
- `Installer/DownSort-v1.0.0-win-x64.zip` (80.59 MB)

더 자세한 내용은 [빌드 가이드](Setup/README.md)를 참조하세요.

---

## 데이터 저장 위치

모든 설정과 로그는 로컬에만 저장됩니다:

```
%LocalAppData%\DownSort\
├── settings.json          # 애플리케이션 설정
├── settings.json.bak      # 자동 백업
├── rules.json             # 정리 규칙 (6개 기본 + 사용자 정의)
├── rules.json.bak         # 자동 백업
└── logs.json              # 작업 로그 (최근 100개)
```

---

## 보안 및 프라이버시

- 모든 데이터는 로컬에만 저장
- 네트워크 통신 없음
- 클라우드 동기화 없음
- 텔레메트리 없음
- 삭제 작업은 기본적으로 비활성화 및 2단계 확인

---

## 문제 해결

### 파일이 정리되지 않는 경우
1. 규칙이 활성화되어 있는지 확인
2. 파일이 규칙 조건과 일치하는지 확인
3. 파일이 잠겨있지 않은지 확인

### 권한 오류
- 애플리케이션을 관리자 권한으로 실행
- 대상 폴더에 쓰기 권한 확인

### 성능 문제
- 설정에서 동시 처리 작업 수 조정
- 감시하는 폴더 수 줄이기

더 많은 문제 해결 방법은 [문제 해결 가이드](Setup/TROUBLESHOOTING.md)를 참조하세요.

---

## 테스트 커버리지

| 프로젝트 | 테스트 수 | 커버리지 |
|---------|----------|---------|
| DownSort.Domain | 8개 | 90%+ |
| DownSort.Infrastructure | 15개 | 85%+ |
| **전체** | **23개** | **85%+** |

주요 테스트:
- RuleEngine (매칭, 우선순위, 템플릿)
- FileOpService (이동, 복사, 삭제, 충돌)
- UndoService (단일/다중 Undo)
- FileWatcherService (실시간 감시)
- 엣지 케이스 및 오류 처리

---

## 라이선스

이 프로젝트는 [MIT 라이선스](LICENSE.txt) 하에 배포됩니다.

---

## 기여

버그 리포트 및 기능 제안은 [GitHub Issues](https://github.com/BaeTab/downsort/issues)를 통해 제출해주세요.

### 개발 가이드
1. 이 저장소를 Fork
2. 기능 브랜치 생성 (`git checkout -b feature/AmazingFeature`)
3. 변경사항 커밋 (`git commit -m 'Add some AmazingFeature'`)
4. 브랜치에 Push (`git push origin feature/AmazingFeature`)
5. Pull Request 생성

---

## 제작

- **아키텍처**: Clean Architecture (Domain-Driven Design)
- **UI 프레임워크**: DevExpress WPF Controls
- **디자인**: Material Design
- **개발 도구**: Visual Studio 2022
- **버전 관리**: Git
- **개발자**: BaeTab

---

## 변경 이력

### v1.0.0 (2024-11-01) - 초기 릴리스

#### 추가
- 기본 파일 정리 기능
- 6개 기본 규칙 (Documents, Images, Archives, Videos, Audio, Installers)
- 규칙 엔진 (조건 매칭, 우선순위)
- 실시간 파일 감시
- Undo 기능 (최대 100개)
- Recent Activity 패널 (실시간 로그, 폴더 바로 가기)
- Material Design UI
- Windows 설치 프로그램 (Inno Setup)

#### 기술
- .NET 8 + WPF
- DevExpress 25.1
- MVVM 패턴
- 85%+ 테스트 커버리지

더 자세한 내용은 [CHANGELOG.md](CHANGELOG.md)를 참조하세요.

---

## 지원

- [사용자 가이드](USER_GUIDE.md)
- [문제 해결](Setup/TROUBLESHOOTING.md)
- [GitHub Issues](https://github.com/BaeTab/downsort/issues)
- [Releases](https://github.com/BaeTab/downsort/releases)
- 이메일: b_h_woo@naver.com

---

## Star History

이 프로젝트가 유용했다면 Star를 눌러주세요!

[![Star History Chart](https://api.star-history.com/svg?repos=BaeTab/downsort&type=Date)](https://star-history.com/#BaeTab/downsort&Date)

---

<div align="center">

Made with :heart: by BaeTab

[맨 위로 이동](#downsort)

</div>
