using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EServerConfig
{
	/// <summary> 문의처 이메일 </summary>
	inquiry_email = 0,
	/// <summary> 문의처 회사 이름 </summary>
	inquiry_company_name,
	/// <summary> 서버 타임존 동기화용 utc time HH:mm:ss </summary>
	server_time_zone,
	/// <summary> 0:서버구동중, 1:점검중 </summary>
	server_state,
	/// <summary> 검검 시작 시간 yyyy-MM-dd HH:mm:ss </summary>
	server_check_stime,
	/// <summary> 점검 종료 시간 yyyy-MM-dd HH:mm:ss </summary>
	server_check_etime,
	/// <summary> 공지 URL </summary>
	Notice_URL,
	/// <summary> 이용약관 URL </summary>
	Terms_of_Service_url,
	/// <summary> 개인정보취급방침 URL </summary>
	Privacy_Policy_url,
	/// <summary> 소비자 보호법 URL </summary>
	Offer_url,
	/// <summary> 공식까페 URL </summary>
	Community_url,
	/// <summary> 피드백 URL </summary>
	PeedBack_url,
	/// <summary> 마켓 url </summary>
	market_url,
	/// <summary> CDN url </summary>
	CDN_url,
	/// <summary> 동영상 url </summary>
	Vedeo_url,
	/// <summary> 접속 가능한 클라이언트 버전 코드. 버전 이후부터 접속 가능 </summary>
	client_ver,
	/// <summary> 커뮤니티 (페이스북) </summary>
	Facebook_url,
	/// <summary> 커뮤니티 (디스코드) </summary>
	Discode_url,
	/// <summary> 커뮤니티 (트위터) </summary>
	Twitter_url,
	/// <summary> 광고 모드 (0: 테스트, 1: 라이브) </summary>
	ADsMode,
	End
}
