args <- commandArgs(TRUE)
filter <-
    c(
        ' ',
        '-',
        '"',
        '\\]',
        '[~!@#$%&*()_+=?<>,.·‘’“♥"/\']',
        '\\[',
        '(ㅜ|ㅠ)',
        '[ㄱ-ㅎ]',
        '\\d{2,}',
        'URL',
        '관련주',
        'ipo',
        '외국인',
        '가능성',
        '대표적',
        '경제뉴스',
        '뉴욕증시',
        '사업자',
        '성수기',
        '순매수',
        '지난해',
        '가운데',
        '기업정보',
        '당기순이익',
        '대부분',
        '대주주',
        '투자하',
        '포스팅에서는',
        'blog',
        '기대감',
        '매출액',
        '영업이익',
        '시가총액',
        '자회사',
        'etf',
        '약억원',
        '억원전년대비',
        'american',
        '실시간',
        '자리매김',
        '존버하라고',
        '지저분',
        '뀨린',
        'https',
        'http',
        'www',
        '상승세',
        '투자자',
        'kodex',
        '금요일',
        '나스닥',
        '지난주',
        'invest',
        'ceo',
        'com',
        '거래대금',
        '경쟁력',
        '미국에서',
        '비수기',
        '발행주식수유동비',
        '상반기',
        '상한가',
        '수익률',
        '순이익',
        '포스팅',
        '계열회사',
        '긍정적',
        '년월일',
        '대형주',
        '마무리',
        '매수세',
        '모회사',
        '보유율',
        '부양책',
        'ETF',
        'KODEX',
        'EPS',
        'INVEST',
        'IPO',
        'CEO',
        'BPS',
        'PBR',
        '컨센서스',
        '경기회복',
        '발행주식수',
        '유동주식비율',
        'PER',
        '기업인수목적회사',
        'spac',
        '스팩',
        '시총',
        '신사업',
        '유가증권',
        '전년동월',
        '주관사',
        '주식회사',
        '주요사업',
        '중소형주',
        '하반기',
        'post',
        'POST',
        '그동안',
        '급등세',
        '네이버증권',
        '대규모',
        '미국',
        '배당주',
        '사용자',
        '수요',
        '오랜만',
        '우리나라',
        '한국거래소',
        'PDF',
        'pdf',
        '뉴욕증권거래소',
        '주식',
        '증시',
        '보통주',
        '불가능',
        '사업보고서',
        '사외이사',
        '상장사',
        '수수료',
        '순매도',
        '시총',
        '였습니다',
        '연평균',
        '왜냐하',
        '이사회',
        '재무재표',
        '재무부',
        '정도인데요',
        'u947f',
        'u4e95',
        '착정',
        '한동안',
        '한국의',
        "는",
        "를",
        "입니다",
        "합니다",
        'ㅜㅜ',
        'ㅇㅇ',
        'read',
        '이네여',
        '것',
        '이구여',
        '있으며',
        'KRX',
        'TIGER',
        'news',
        '꼴찌',
        '주린이'
    )
lapply(
    c(
        "rJava",
        "KoNLP",
        "multilinguer",
        "hash",
        "tau",
        "Sejong",
        "RSQLite",
        "devtools",
        "bit",
        "rex",
        "lazyeval",
        "htmlwidgets",
        "crosstalk",
        "promises",
        "later",
        "sessioninfo",
        "xopen",
        "bit64",
        "blob",
        "DBI",
        "memoise",
        "plogr",
        "covr",
        "DT",
        "rcmdcheck",
        "rversions",
        "wordcloud",
        "wordcloud2",
        "RColorBrewer",
        "tm",
        "stringr",
        "SnowballC",
        "webshot",
        "NIADic",
        "remotes"
    ),
    require,
    character.only = TRUE
)
useSejongDic()
codes <-
    read.csv(
        paste(args[2], '.csv', sep = '' , collapse = ''),
        sep = "\t",
        header =  FALSE,
        fileEncoding = 'UTF-8',
        stringsAsFactors = FALSE
    )
except <- data.frame(c('삼성SDS', '2차전지', '5G', '4차산업'), 'ncn')
mergeUserDic(codes)
mergeUserDic(except)
words <-
    unlist(sapply(readLines(
        paste(args[1], '.txt', sep = '', collapse = ''), encoding = 'UTF-8'
    ), extractNoun, USE.NAMES = F))
words <- Filter(function(x) {
    nchar(x) > 1
}, words)

for (i in 1:length(filter))
{
    words <- gsub(filter[i], '', words)
}
for (code in codes$V1)
{
    words <-
        gsub(paste(code, '\\S*', sep = '', collapse = ''), code, words)
}
for (ex in except[, 1])
{
    words <-
        gsub(paste(ex, '\\S*', sep = '', collapse = ''), ex, words)
}
options(mc.cores = 1)
tm.words <- VCorpus(VectorSource(words))
tm.words <- tm_map(tm.words, stripWhitespace)
tm.words <- tm_map(tm.words, removePunctuation)
matrix <-
    sort(rowSums(as.matrix(TermDocumentMatrix(tm.words))), decreasing = TRUE)
data <- data.frame(word = toupper(names(matrix)), freq = matrix)
write.csv(
    data[data$freq > 2, ],
    paste(args[1], '.csv', sep = '' , collapse = ''),
    row.names = FALSE,
    fileEncoding = 'UTF-8'
)
warnings()