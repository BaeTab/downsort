# GitHub Actions 자동 빌드 및 릴리스 가이드

## 개요

이 프로젝트는 GitHub Actions를 사용하여 자동 빌드, 테스트, 릴리스를 수행합니다.

---

## 워크플로우

### 1. CI (Continuous Integration)
**파일**: `.github/workflows/ci.yml`

#### 트리거
- `master`, `main`, `develop` 브랜치에 push
- Pull Request 생성

#### 작업
1. 코드 체크아웃
2. .NET 8 SDK 설정
3. NuGet 패키지 복원 (캐싱)
4. 솔루션 빌드
5. 단위 테스트 실행 (23개)
6. 테스트 결과 업로드 (TRX 형식)
7. 빌드 아티팩트 업로드

#### 최신 업데이트
- ? `actions/checkout@v4`
- ? `actions/setup-dotnet@v4`
- ? `actions/cache@v4`
- ? `actions/upload-artifact@v4` (deprecated v3에서 업그레이드)

### 2. Release (자동 릴리스)
**파일**: `.github/workflows/release.yml`

#### 트리거
- Git 태그 생성 (예: `v1.0.0`, `v1.2.3`)
- 수동 실행 (workflow_dispatch)

#### 작업
1. 빌드 및 테스트
2. Directory.Build.props 버전 업데이트
3. 애플리케이션 게시 (publish)
4. Inno Setup 설치
5. Windows 설치 프로그램 생성
6. ZIP 아카이브 생성
7. 체크섬 계산
8. GitHub Release 자동 생성
9. 파일 자동 업로드

---

## 릴리스 생성 방법

### 옵션 1: Git 태그로 자동 릴리스 (권장)

```bash
# 1. Directory.Build.props 버전 업데이트 (선택사항)
# <Version>1.0.1</Version>

# 2. 변경사항 커밋
git add .
git commit -m "Release v1.0.1"
git push origin master

# 3. 태그 생성
git tag -a v1.0.1 -m "Release v1.0.1"

# 4. 태그 푸시
git push origin v1.0.1
```

**결과**: GitHub Actions가 자동으로 실행되어 릴리스 생성

### 옵션 2: 수동 실행

1. GitHub 리포지토리로 이동
2. **Actions** 탭 클릭
3. **Build and Release** 워크플로우 선택
4. **Run workflow** 버튼 클릭
5. 브랜치 선택 후 실행

---

## 버전 번호 규칙

### Semantic Versioning (SemVer)
```
v{Major}.{Minor}.{Patch}
```

**예제**:
- `v1.0.0` - 초기 릴리스
- `v1.0.1` - 버그 수정 (Patch)
- `v1.1.0` - 새 기능 추가 (Minor)
- `v2.0.0` - 주요 변경 (Major)

### 버전 증가 가이드

| 변경 유형 | 버전 증가 | 예시 |
|----------|----------|------|
| 버그 수정 | Patch | 1.0.0 → 1.0.1 |
| 새 기능 (하위 호환) | Minor | 1.0.0 → 1.1.0 |
| 주요 변경 (하위 비호환) | Major | 1.0.0 → 2.0.0 |

---

## 중앙 버전 관리

### Directory.Build.props
프로젝트 루트의 `Directory.Build.props` 파일에서 버전을 중앙 관리합니다:

```xml
<Project>
  <PropertyGroup>
    <Version>1.0.0</Version>
    <AssemblyVersion>1.0.0.0</AssemblyVersion>
    <FileVersion>1.0.0.0</FileVersion>
    <InformationalVersion>1.0.0</InformationalVersion>
  </PropertyGroup>
</Project>
```

#### 장점
- ? 한 곳에서 모든 프로젝트 버전 관리
- ? 빌드 스크립트가 자동으로 버전 읽기
- ? GitHub Actions가 태그 버전으로 자동 업데이트
- ? 일관된 버전 번호 보장

---

## 워크플로우 상세 설명

### 1단계: 빌드 및 테스트
```yaml
- Checkout code
- Setup .NET 8
- Cache NuGet packages (v4)
- Restore dependencies
- Build solution
- Run tests (23개 단위 테스트)
- Upload test results (v4)
- Upload build artifacts (v4)
```

### 2단계: 버전 관리
```yaml
- Get version from tag (v1.0.1)
- Update Directory.Build.props
- Update ISS file version
```

### 3단계: 애플리케이션 게시
```yaml
- Clean previous builds
- Restore with runtime
- Publish for win-x64
- Single file: true
- Self-contained: false
```

### 4단계: 설치 프로그램 생성
```yaml
- Install Inno Setup via Chocolatey
- Compile installer
- Output: DownSort-Setup-{version}.exe
```

### 5단계: 아카이브 생성
```yaml
- Create ZIP from published files
- Add documentation files
- Output: DownSort-v{version}-win-x64.zip
```

### 6단계: 체크섬 계산
```yaml
- Calculate SHA256 for installer
- Calculate SHA256 for ZIP
- Save to checksums.txt
```

### 7단계: GitHub Release
```yaml
- Create release with version tag
- Upload installer (.exe)
- Upload ZIP archive
- Upload checksums
- Upload Directory.Build.props
- Auto-generate release notes
```

---

## 릴리스 확인

### 1. GitHub에서 확인
```
https://github.com/BaeTab/downsort/releases
```

### 2. Actions 로그 확인
```
https://github.com/BaeTab/downsort/actions
```

### 3. 다운로드 테스트
```
1. Release 페이지에서 .exe 다운로드
2. 설치 및 실행 테스트
3. 체크섬 검증 (선택)
```

---

## 체크섬 검증 방법

### Windows PowerShell
```powershell
# SHA256 계산
Get-FileHash -Path "DownSort-Setup-1.0.0.exe" -Algorithm SHA256

# 결과를 checksums.txt와 비교
```

### 명령 프롬프트
```cmd
certutil -hashfile "DownSort-Setup-1.0.0.exe" SHA256
```

---

## 문제 해결

### 워크플로우 실패 시

#### 1. 빌드 실패
```
원인: 코드 오류, 의존성 문제
해결: 로컬에서 빌드 테스트 후 수정
```

#### 2. 테스트 실패
```
원인: 단위 테스트 실패
해결: 로컬에서 dotnet test 실행 후 수정
```

#### 3. Inno Setup 실패
```
원인: ISS 스크립트 오류
해결: Setup/DownSort.iss 확인
```

#### 4. 권한 오류
```
원인: GITHUB_TOKEN 권한 부족
해결: Repository Settings > Actions > General > Workflow permissions
      "Read and write permissions" 선택
```

#### 5. Deprecated Actions (v3 → v4)
```
원인: actions/upload-artifact@v3 사용
해결: v4로 업그레이드 완료 (2024-01-10)
```

---

## 최근 업데이트 (2024-01-15)

### GitHub Actions v4 업그레이드
- ? `actions/checkout@v4`
- ? `actions/setup-dotnet@v4`
- ? `actions/cache@v4` (v3에서 업그레이드)
- ? `actions/upload-artifact@v4` (v3 deprecated 대응)

### 변경 사항
```yaml
# 이전 (deprecated)
- uses: actions/upload-artifact@v3

# 현재 (최신)
- uses: actions/upload-artifact@v4
```

---

## 고급 설정

### 1. 자동 버전 번호 증가

현재는 수동으로 태그를 생성하지만, 다음과 같이 자동화할 수 있습니다:

```yaml
# .github/workflows/auto-version.yml
- name: Bump version
  uses: anothrNick/github-tag-action@1.64.0
  env:
    GITHUB_TOKEN: ${{ secrets.GITHUB_TOKEN }}
    WITH_V: true
    DEFAULT_BUMP: patch
```

### 2. Pre-release 생성

```bash
# 베타 버전 태그
git tag -a v1.1.0-beta.1 -m "Beta release"
git push origin v1.1.0-beta.1
```

워크플로우에서 자동으로 pre-release로 표시됩니다.

### 3. 릴리스 노트 자동 생성

`CHANGELOG.md`를 자동으로 읽어 릴리스 노트에 포함:

```yaml
- name: Extract changelog
  run: |
    $changelog = Get-Content "CHANGELOG.md" -Raw
    # 버전별로 파싱
```

---

## 베스트 프랙티스

### 1. 태그 생성 전 체크리스트
- [ ] 모든 테스트 통과 확인
- [ ] `CHANGELOG.md` 업데이트
- [ ] `Directory.Build.props` 버전 확인
- [ ] 로컬에서 빌드 테스트
- [ ] 로컬에서 설치 프로그램 생성 테스트

### 2. 커밋 메시지 규칙
```
feat: 새 기능 추가
fix: 버그 수정
docs: 문서 변경
style: 코드 포맷팅
refactor: 리팩토링
test: 테스트 추가
chore: 빌드/배포 관련
```

### 3. 브랜치 전략
```
master/main  - 안정 버전
develop      - 개발 버전
feature/*    - 기능 개발
hotfix/*     - 긴급 수정
```

---

## 실전 예제

### 시나리오 1: 버그 수정 릴리스

```bash
# 1. 버그 수정 커밋
git add .
git commit -m "fix: 파일 정리 오류 수정"
git push origin master

# 2. Directory.Build.props 업데이트 (1.0.0 → 1.0.1)
# <Version>1.0.1</Version>

# 3. 버전 태그 생성
git tag -a v1.0.1 -m "버그 수정 릴리스"
git push origin v1.0.1

# 4. GitHub Actions 자동 실행
# 5. Release 페이지에 v1.0.1 자동 생성
```

### 시나리오 2: 새 기능 릴리스

```bash
# 1. 기능 개발 완료
git add .
git commit -m "feat: 새로운 규칙 엔진 추가"
git push origin master

# 2. CHANGELOG.md 업데이트
# 3. Directory.Build.props 업데이트 (1.0.1 → 1.1.0)
git tag -a v1.1.0 -m "새 기능 릴리스"
git push origin v1.1.0

# 4. 자동 릴리스 생성
```

---

## 로컬 테스트

GitHub Actions를 실행하기 전에 로컬에서 테스트:

```powershell
# 1. 빌드 테스트
dotnet build DownSort.sln --configuration Release

# 2. 테스트 실행
dotnet test DownSort.sln --configuration Release

# 3. 게시 테스트
dotnet publish Downsort/Downsort.csproj --configuration Release --runtime win-x64

# 4. 설치 프로그램 생성
cd Setup
.\build-installer.ps1 -CreateInstaller
```

---

## 모니터링

### GitHub Actions 대시보드
```
Repository > Actions 탭
- 실행 중인 워크플로우 확인
- 로그 실시간 모니터링
- 실패 원인 분석
```

### 이메일 알림
```
Settings > Notifications
- Workflow 실패 시 이메일 수신
```

---

## 참고 자료

- [GitHub Actions 문서](https://docs.github.com/actions)
- [Semantic Versioning](https://semver.org/)
- [Inno Setup](https://jrsoftware.org/isinfo.php)
- [.NET CLI](https://docs.microsoft.com/dotnet/core/tools/)
- [GitHub Actions v4 Migration](https://github.blog/changelog/2024-04-16-deprecation-notice-v3-of-the-artifact-actions/)

---

**작성일**: 2024-11-01  
**최종 업데이트**: 2024-01-15  
**버전**: 1.1  
**작성자**: BaeTab
