***khách tạo đơn (createReq)
*1/bấm "tôi đã thanh toán"  hiện trên màng hình "Đã thanh toán, Nhân viên đang đến nhận kim cương!"(PaymentController) reqstatus
khách tạo đơn (createReq)
-> xem được đơn đã tạo(History req ) condition CustomerId đang đăng nhập

--> tạo thành công thì nhân viên xem đc (getall with Req status="đã thanh toán") done
--> chấp nhận đơn hàng(list req get all & bấm nut accept thì check token Employee!=null  set EmoyeeID=Eployee đang có token)  done                          
--History of request(get  by Id)
 --> nhảy qua tab những thứ đã chấp nhận của nhân viên--getallReq with status"đã nhận kim cương -EmployeeId" (tab này có 1 nút để tạo kết quả) oke--HistoryController
--> nhập kết quả oke
--> nhảy qua tab danh sách kết quả (có nút để gửi cho admin)oke
admin:
-tab(acccout)oke
-list Result ở Admin page done
change status "Kiểm định thành công"\ done


status:(Reqstatus)
-"Đã thanh toán"--sau khi bấm "tôi đã thanh toán"  (chỉnh trong Request)
-"Đã nhận kim cương"--sau khi nhiên accept req
-"Kiểm định thành công"--sau khi admin bấm"kiểm định thành công" && deny - reqstatus:"Đã nhận kim cương"()
-"Đã giao kim cương thành công"-->sau khi nhân viên bấm "đã giao kim cương"
-"Kim cương đã  niêm phong"--khách ko nhận sau n ngày oke (tính theo IssueDate của Certifiacte)
