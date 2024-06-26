*1/bấm "tôi đã thanh toán"  hiện trên màng hình "Đã thanh toán, Nhân viên đang đến nhận kim cương!"(PaymentController) reqstatus
khách tạo đơn (createReq)
-> xem được đơn đã tạo(History req ) condition CustomerId đang đăng nhập

--> tạo thành công thì nhân viên xem đc (getall with Req status="đã thanh toán") done
--> chấp nhận đơn hàng()??
 --> nhảy qua tab những thứ đã chấp nhận--getallReq with status"đã nhận kim cương" (tab này có 1 nút để tạo kết quả) oke--HistoryController
--> nhập kết quả oke
--> nhảy qua tab danh sách kết quả (có nút để gửi cho admin)oke
admin:
-tab(acccout)oke
-list Result ở Admin page done
change status "Kiểm định thành công"\ done


status:(Reqstatus)
-"Đã thanh toán"--sau khi bấm "tôi đã thanh toán"
-"Đã nhận kim cương"--sau khi nhiên accept req
-"Kiểm định thành công"--sau khi admin bấm"kiểm định thành công"
-"Đã giao kim cương thành công"-->sau khi nhân viên bấm "đã giao kim cương"
-"Kim cương đã  niêm phong"--khách ko nhận sau n ngày oke
