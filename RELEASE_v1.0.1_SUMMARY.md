# v1.0.1 릴리스 생성 완료!

## ?? 태그 푸시 성공!

### ?? 태그 정보
```
Tag: v1.0.1
Message: Release v1.0.1 - Custom title bar and DevExpress support
Status: Pushed to origin
```

---

## ?? GitHub Actions 자동 실행 중

### 1. Release 워크플로우 실행 중
```
https://github.com/BaeTab/downsort/actions
```

**예상 작업 순서** (약 5-10분 소요):
1. ? Checkout code
2. ? Setup .NET 8
3. ? Setup DevExpress NuGet source (with secrets)
4. ? Restore dependencies
5. ? Build solution
6. ? Run tests
7. ? Update Directory.Build.props version
8. ? Publish application
9. ? Install Inno Setup
10. ? Create installer (DownSort-Setup-1.0.1.exe)
11. ? Create ZIP archive
12. ? Calculate checksums
13. ? Create GitHub Release
14. ? Upload files

---

## ?? 릴리스 파일

### 생성될 파일 (자동)
1. **DownSort-Setup-1.0.1.exe** - Windows 설치 프로그램
2. **DownSort-v1.0.1-win-x64.zip** - Portable 버전
3. **checksums.txt** - SHA256 체크섬
4. **Directory.Build.props** - 버전 정보

---

## ?? 확인 링크

### GitHub Actions
```
https://github.com/BaeTab/downsort/actions
```
- **Workflow**: Build and Release
- **Trigger**: Tag push (v1.0.1)
- **Status**: 실행 중...

### GitHub Releases (5-10분 후)
```
https://github.com/BaeTab/downsort/releases
```
- **v1.0.1** 릴리스가 자동 생성됨
- 설치 프로그램 및 ZIP 파일 다운로드 가능

---

## ?? 릴리스 내용

### v1.0.1 변경 사항

#### 새 기능
- ? Material Design 커스텀 타이틀바
- ? 드래그 & 드롭으로 창 이동
- ? 더블클릭 최대화/복원
- ? 호버 효과 (파란색/빨간색)

#### 개선 사항
- ? GitHub Actions v4 업그레이드
- ? DevExpress NuGet feed 설정
- ? 중앙 버전 관리 (Directory.Build.props)
- ? 상세 문서 추가

#### 버그 수정
- ? InvalidCastException 수정
- ? 네임스페이스 충돌 해결
- ? NU1101/NU1102 패키지 복원 오류 수정

---

## ?? 릴리스 확인 방법

### 1. GitHub Actions 실행 확인
```sh
# 브라우저에서 확인
https://github.com/BaeTab/downsort/actions

# 또는 터미널에서
gh run list --workflow=release.yml
```

### 2. 릴리스 페이지 확인 (5-10분 후)
```
https://github.com/BaeTab/downsort/releases/tag/v1.0.1
```

### 3. 설치 프로그램 다운로드
- **DownSort-Setup-1.0.1.exe** 클릭
- Windows에서 실행 및 테스트

---

## ?? 릴리스 워크플로우 단계별 상태

### Build and Test (1단계)
- [ ] Checkout code
- [ ] Setup .NET
- [ ] Setup DevExpress NuGet source
- [ ] Restore dependencies
- [ ] Build solution
- [ ] Run tests

### Create Release (2단계)
- [ ] Get version from tag (v1.0.1)
- [ ] Update Directory.Build.props
- [ ] Clean previous builds
- [ ] Restore with runtime (win-x64)
- [ ] Build solution
- [ ] Publish application
- [ ] Verify published files
- [ ] Install Inno Setup
- [ ] Create installer
- [ ] Create ZIP archive
- [ ] Calculate checksums
- [ ] Generate release notes
- [ ] Create GitHub Release
- [ ] Upload files

---

## ?? 릴리스 노트 (자동 생성)

```markdown
# DownSort v1.0.1

## 다운로드

### Windows 설치 프로그램 (권장)
- **DownSort-Setup-1.0.1.exe** - Windows Installer
- 자동 설치 및 설정
- .NET 8 Runtime 자동 체크

### 수동 설치 (ZIP)
- **DownSort-v1.0.1-win-x64.zip** - Portable version
- 압축 해제 후 바로 실행

## 시스템 요구사항
- Windows 10 (64-bit) 이상
- .NET 8 Runtime

## 체크섬
파일 무결성 확인을 위한 SHA256 해시는 checksums.txt를 참조하세요.

## 문서
- [사용자 가이드](https://github.com/BaeTab/downsort/blob/master/USER_GUIDE.md)
- [문제 해결](https://github.com/BaeTab/downsort/blob/master/Setup/TROUBLESHOOTING.md)
- [변경 이력](https://github.com/BaeTab/downsort/blob/master/CHANGELOG.md)
```

---

## ?? 문제 해결

### Q1: 워크플로우가 실행되지 않음
**확인**:
- Actions 탭에서 "Build and Release" 워크플로우 확인
- 태그가 올바르게 푸시되었는지 확인: `git tag -l`

### Q2: 빌드 실패 (DevExpress 오류)
**확인**:
- GitHub Secrets가 올바르게 설정되었는지 확인
- `DEVEXPRESS_NUGET_USERNAME` 
- `DEVEXPRESS_NUGET_KEY`

### Q3: 릴리스가 생성되지 않음
**확인**:
- Workflow permissions 확인:
```
Settings > Actions > General > Workflow permissions
"Read and write permissions" 선택
```

---

## ?? 진행 상황 모니터링

### 실시간 로그 확인
```
https://github.com/BaeTab/downsort/actions
→ 최신 workflow 클릭
→ "Create Release" job 클릭
→ 각 step의 로그 확인
```

### 예상 시간
- **빌드 & 테스트**: 2-3분
- **릴리스 생성**: 3-5분
- **총 소요 시간**: 5-10분

---

## ? 다음 단계

### 1. Actions 확인 (지금)
```
https://github.com/BaeTab/downsort/actions
```

### 2. 릴리스 확인 (5-10분 후)
```
https://github.com/BaeTab/downsort/releases/tag/v1.0.1
```

### 3. 설치 프로그램 테스트
- Setup 파일 다운로드
- Windows에서 설치 테스트
- 커스텀 타이틀바 확인

---

## ?? 완료!

**v1.0.1 릴리스가 자동으로 생성됩니다!**

- 태그: `v1.0.1` ?
- Actions: 실행 중... ?
- Release: 생성 예정... ?

**5-10분 후 릴리스 페이지에서 확인하세요!**

---

**작성일**: 2024-01-10  
**태그**: v1.0.1  
**상태**: GitHub Actions 실행 중
