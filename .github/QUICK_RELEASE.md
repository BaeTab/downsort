# GitHub Actions 빠른 참조

## 새 릴리스 생성 (3단계)

```bash
# 1. 코드 커밋 및 푸시
git add .
git commit -m "Release v1.0.1"
git push origin master

# 2. 태그 생성
git tag -a v1.0.1 -m "Release v1.0.1"

# 3. 태그 푸시 (자동 릴리스 시작!)
git push origin v1.0.1
```

**결과**: 5-10분 후 https://github.com/BaeTab/downsort/releases 에서 확인

---

## 버전 번호 규칙

| 변경 | 이전 | 새 버전 |
|------|------|---------|
| 버그 수정 | v1.0.0 | v1.0.1 |
| 새 기능 | v1.0.0 | v1.1.0 |
| 주요 변경 | v1.0.0 | v2.0.0 |

---

## 자동 생성되는 파일

- `DownSort-Setup-{version}.exe` - Windows 설치 프로그램
- `DownSort-v{version}-win-x64.zip` - 수동 설치용
- `checksums.txt` - SHA256 체크섬

---

## 문제 해결

### 워크플로우 확인
```
https://github.com/BaeTab/downsort/actions
```

### 로컬 테스트
```powershell
dotnet test
cd Setup
.\build-installer.ps1 -CreateInstaller
```

### 태그 삭제 (실수한 경우)
```bash
# 로컬 태그 삭제
git tag -d v1.0.1

# 원격 태그 삭제
git push --delete origin v1.0.1
```

---

## 체크리스트

릴리스 전:
- [ ] `dotnet test` 통과
- [ ] `CHANGELOG.md` 업데이트
- [ ] `README.md` 버전 확인
- [ ] 로컬 빌드 테스트

---

**더 자세한 내용**: [RELEASE_GUIDE.md](RELEASE_GUIDE.md)
